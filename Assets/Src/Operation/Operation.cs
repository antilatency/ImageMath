using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


#if UNITY_EDITOR
using UnityEditor;
#endif

#nullable enable
namespace ImageMath {

    public class GeneratedAttribute : Attribute {
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class FilePathAttribute : Attribute {
        public string FilePath { get; }
        public FilePathAttribute([System.Runtime.CompilerServices.CallerFilePath] string filePath = "") {
            FilePath = filePath;
        }
    }

    public abstract record Operation {

        public static string GetShaderFileName(ClassDescription classDescription) => classDescription.Type.FullName+".shader"; 

        protected virtual void ApplyShaderParameters() {
        }


        #if UNITY_EDITOR

        protected static string LoadCode(string? name = null, [System.Runtime.CompilerServices.CallerFilePath] string filePath = "") {
            if (string.IsNullOrEmpty(name)) {
                name = System.IO.Path.GetFileNameWithoutExtension(filePath);
            }
            if (!Path.HasExtension(name)){
                name += ".cginc";
            }
            string path = Path.Combine(Path.GetDirectoryName(filePath)!, name);
            var content = File.ReadAllText(path);
            return content;
        }
        protected static string Include(string? name = null, [System.Runtime.CompilerServices.CallerFilePath] string filePath = "") {
            if (string.IsNullOrEmpty(name)) {
                name = System.IO.Path.GetFileNameWithoutExtension(filePath);
            }
            if (!Path.HasExtension(name)){
                name += ".cginc";
            }
            string path = Path.Combine(Path.GetDirectoryName(filePath)!, name);
            return $"#include \"{path}\"";
        }

        

        /*public static void CollectIncludes(List<string> includes) {

        }*/

        public static string GetCustomCodeCombined(ClassDescription classDescription) {
            var hierarchy = classDescription.GetHierarchy().ToArray();
            var includes = new List<string>();
            for (int i = hierarchy.Length - 1; i >= 0; i--) {
                var type = hierarchy[i];
                var method = type.GetMethod("GetCustomCode", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                if (method != null) {
                    method.Invoke(null, new object[] { includes });
                }
            }
            return "";
        }      

        public static string GetConstants(){
            var result = new StringBuilder();
            result.AppendLine("#define Pi 3.1415926535897932384626433832795");
            result.AppendLine("#define SquareRootOf2 1.4142135623730950488016887242097");
            result.AppendLine("#define Epsilon 10e-6");
            return result.ToString();
        }

        #endif

    }
}
