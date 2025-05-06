using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Scopes;
using Scopes.C;
using UnityEditor;
using UnityEngine;
#nullable enable
namespace ImageMath{

    internal static class PathExtensions {
        public static bool IsSubPathOf(this string path, string baseDirPath) {
            string normalizedPath = Path.GetFullPath(path.Replace('/', '\\').TrimEnd('\\'));
            string normalizedBaseDirPath = Path.GetFullPath(baseDirPath.Replace('/', '\\').TrimEnd('\\'));

            if (string.Equals(normalizedPath, normalizedBaseDirPath, StringComparison.OrdinalIgnoreCase)) {
                return true;
            }
            return normalizedPath.StartsWith(normalizedBaseDirPath+Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
        }
    }

    public class Generator {

        [MenuItem("ImageMath/Delete Generated Files")]
        public static void DeleteGeneratedFiles(){
            var directoriesToDelete = Directory.EnumerateDirectories(Application.dataPath, ImageMathGeneratedDirectoryName, SearchOption.AllDirectories).ToList();
            directoriesToDelete.ForEach(d => {
                if (d != null){
                    Directory.Delete(d, true);
                }
            });
        }

        [MenuItem("ImageMath/Generate %G")]
        public static void Generate(){
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            ClassDescription opeationClass = new ClassDescription(typeof(Operation), null);

            DeleteGeneratedFiles();

            try{
                foreach (var assembly in assemblies){
                    var types = assembly.GetTypes();
                    //Debug.Log($"Assembly: {assembly.FullName}");
                    foreach (var type in types){
                        if (type.IsClass && type.IsSubclassOf(typeof(Operation))){
                            opeationClass.FindOrCreate(type);                       
                        }
                    }
                }

                var flattened = new List<ClassDescription>();
                opeationClass.FlattenChildren(flattened);
                foreach (var classDescription in flattened){
                    var type = classDescription.Type;
                    var filePath = GetFilePath(type);
                    if (filePath == null){
                        Debug.LogError($"FilePathAttribute not found for {type.Name}");
                        continue;
                    }
                    var isFileInsideAssets = filePath.IsSubPathOf(Application.dataPath);
                    if (isFileInsideAssets){
                        GenerateCsPartial(classDescription);
                        if (!type.IsAbstract){
                            GenerateShaderForType(classDescription);
                        }
                    }               
                }
            } catch (Exception e){
                Debug.LogError(e.Message);
            }

            

            AssetDatabase.Refresh();
        }

        public static string? GetFilePath(Type type){
            var filePathAttribute = type.GetCustomAttribute<FilePathAttribute>();
            if (filePathAttribute == null){
                return null;
            }
            var filePath = filePathAttribute.FilePath;
            return filePath;
        }


        const string ImageMathGeneratedDirectoryName = "ImageMathGenerated";

        public static string GetPathFromType(Type type, string extension){
            var namespaceElements = string.IsNullOrEmpty(type.Namespace)
            ? Enumerable.Empty<string>()
            : type.Namespace.Split('.');

            var assetsDirectory = Application.dataPath;

            var pathElements = namespaceElements.Prepend("Resources").Prepend(ImageMathGeneratedDirectoryName).Prepend(assetsDirectory).Append(type.Name+extension).ToArray();
            string path = Path.Combine(pathElements);
            return path;
        }

        public static void GenerateCsPartial(ClassDescription classDescription){
            var applyShaderParametersGroup = new Scope("protected override void ApplyShaderParameters()"){
                "base.ApplyShaderParameters();",
                classDescription.Parameters.Select(p => p.GetShaderParameterAssignmentCode())
            };


            var _record = new Group{
                "[ImageMath.Generated]",
                new Scope($"public partial record {classDescription.Name}"){
                    applyShaderParametersGroup                    
                }
            };

            var _namespace = classDescription.Namespace;

            var content = new Group() {
                "using UnityEngine;"
                ,!string.IsNullOrEmpty(_namespace)
                ?new Scope($"namespace {_namespace}"){
                    _record
                }
                : _record
            };

            /*var namespaceElements = string.IsNullOrEmpty(hierarchy[0].Namespace)
            ? Enumerable.Empty<string>()
            : hierarchy[0].Namespace.Split('.');

            var pathElements = namespaceElements.Prepend(ImageMathGeneratedDirectoryName).Prepend("Assets").Append(hierarchy[0].Name+".cs").ToArray();
            */
            var mainFilePath = GetFilePath(classDescription.Type);
            if (mainFilePath == null){
                throw new Exception($"FilePathAttribute not found for {classDescription.Type.Name}");
            }
            var fileName = Path.GetFileName(mainFilePath);
            var diretctory = Path.GetDirectoryName(mainFilePath)!;
            var generatedFilePath = Path.Combine(diretctory, ImageMathGeneratedDirectoryName, fileName);

            //string path = GetPathFromType(classDescription.Type, ".cs");
            
            WriteAllText(generatedFilePath, content.ToString());
            
        }

        static void WriteAllText(string path, string content){
            string directory = Path.GetDirectoryName(path)!;
            if (!Directory.Exists(directory)){
                Directory.CreateDirectory(directory);
            }
            File.WriteAllText(path, content);
        }

        public static T CallStaticMethod<T>(ClassDescription classDescription, string methodName){
            var hierarchy = classDescription.GetHierarchy().Append(typeof(Generator));
            //var hierrarhyWithGenerator = hierarchy.ToList();
            //hierrarhyWithGenerator.Add(typeof(Generator));

            foreach (var type in hierarchy) {
                try{
                    var method = type.GetMethod(methodName
                        ,BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic
                        ,null, new Type[]{}, null);
                    if (method != null){
                        return (T)method.Invoke(null, null);
                    } else {
                        method = type.GetMethod(methodName
                            ,BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic
                            ,null, new Type[] { typeof(ClassDescription) }, null);
                        if (method != null){                        
                            return (T)method.Invoke(null, new object[]{classDescription});                        
                        }
                    }
                    
                } catch (Exception e){
                    throw new Exception($"Error calling {methodName} on {type.Name}: {e.Message}");
                }
            }
            throw new Exception($"Method {methodName} not found in hierarchy {string.Join(" -> ", hierarchy.Select(t => t.Name))}");
        }
            

        public static void GenerateShaderForType(ClassDescription classDescription){
            var template = CallStaticMethod<string>(classDescription, "GetTemplate");

            while (true){
                var insert = System.Text.RegularExpressions.Regex.Match(template, @"([\t ]*)@(\w+)");
                if (!insert.Success){
                    break;
                }
                var indent = insert.Groups[1].Value;
                var name = insert.Groups[2].Value;
                var value = CallStaticMethod<string>(classDescription, name);

                var lines = value.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i=0; i < lines.Length; i++){
                    lines[i] = indent + lines[i].TrimEnd();
                }
                value = string.Join("\n", lines);

                template = template.Remove(insert.Index, insert.Length).Insert(insert.Index, value);
            }


            var mainFilePath = GetFilePath(classDescription.Type);
            if (mainFilePath == null){
                throw new Exception($"FilePathAttribute not found for {classDescription.Type.Name}");
            }
            var fileName = CallStaticMethod<string>(classDescription, "GetShaderFileName");
            var diretctory = Path.GetDirectoryName(mainFilePath)!;
            var generatedFilePath = Path.Combine(diretctory, ImageMathGeneratedDirectoryName,"Resources", fileName);

            WriteAllText(generatedFilePath, template);
        }

        public static string GetParameters(ClassDescription classDescription){
            var current = classDescription;
            var stringBuilder = new System.Text.StringBuilder();
            while (current != null){
                var parameters = current.Parameters;
                foreach (var parameter in parameters){
                    var hlslDeclaration = parameter.GetHLSLDeclaration();
                    stringBuilder.AppendLine(hlslDeclaration);
                }
                
                current = current.Parent;
            }
            return stringBuilder.ToString();
        }



    }
}