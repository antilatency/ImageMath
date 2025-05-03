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

    public class Generator {

        [MenuItem("ImageMath/Generate %G")]
        public static void Generate(){
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            ClassDescription opeationClass = new ClassDescription(typeof(Operation), null);


            foreach (var assembly in assemblies){
                var types = assembly.GetTypes();
                //Debug.Log($"Assembly: {assembly.FullName}");
                foreach (var type in types){
                    if (type.IsClass && type.IsSubclassOf(typeof(Operation))){


                        var classDescription = opeationClass.FindOrCreate(type);
                        
                        /*if (type.IsAbstract){
                            continue;
                        }
                        
                        GenerateShaderForType(hierarchy);*/
                        
                    }
                }
            }

            var flattened = new List<ClassDescription>();
            opeationClass.FlattenChildren(flattened);
            foreach (var classDescription in flattened){
                GenerateCsPartial(classDescription);
                if (!classDescription.Type.IsAbstract){
                    GenerateShaderForType(classDescription);
                }                
            }

            AssetDatabase.Refresh();
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


            var _record = new Scope($"public partial record {classDescription.Name}"){
                applyShaderParametersGroup                    
            };
            var _namespace = classDescription.Namespace;
            if (!string.IsNullOrEmpty(_namespace)){
                _record = new Scope($"namespace {_namespace}"){
                    _record
                };
            }

            var content = new Group() {
                "using UnityEngine;"
                ,_record
            };

            /*var namespaceElements = string.IsNullOrEmpty(hierarchy[0].Namespace)
            ? Enumerable.Empty<string>()
            : hierarchy[0].Namespace.Split('.');

            var pathElements = namespaceElements.Prepend(ImageMathGeneratedDirectoryName).Prepend("Assets").Append(hierarchy[0].Name+".cs").ToArray();
            */
            string path = GetPathFromType(classDescription.Type, ".cs");
            
            WriteAllText(path, content.ToString());
            
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
            var path = GetPathFromType(classDescription.Type, ".shader");
            WriteAllText(path, template);
            //Debug.Log(template);
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