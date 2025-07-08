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

        public static string GetShaderFileName(ClassDescription classDescription) => classDescription.Type.FullName + ".shader";

        protected virtual void ApplyShaderParameters() {
        }

        protected virtual void ApplyCustomShaderParameters() {
        }

#if UNITY_EDITOR

        protected static string LoadCode(string? name = null, [System.Runtime.CompilerServices.CallerFilePath] string filePath = "") {
            if (string.IsNullOrEmpty(name)) {
                name = System.IO.Path.GetFileNameWithoutExtension(filePath);
            }
            if (!Path.HasExtension(name)) {
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
            if (!Path.HasExtension(name)) {
                name += ".cginc";
            }
            string path = Path.Combine(Path.GetDirectoryName(filePath)!, name);
            return $"#include \"{path}\"";
        }



        /*public static void CollectIncludes(List<string> includes) {

        }*/

        public static string GetCustomCodeCombined(ClassDescription classDescription) {
            var hierarchy = classDescription.GetHierarchy().ToArray();
            var stringBuilder = new StringBuilder();
            for (int i = hierarchy.Length - 1; i >= 0; i--) {
                var type = hierarchy[i];
                var method = type.GetMethod("GetCustomCode", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                if (method != null) {
                    var code = (string)method.Invoke(null, null);
                    stringBuilder.AppendLine(code);
                }
            }
            return stringBuilder.ToString();
        }

        public static string GetConstants() {
            var result = new StringBuilder();
            result.AppendLine("#define Pi 3.1415926535897932384626433832795");
            result.AppendLine("#define SquareRootOf2 1.4142135623730950488016887242097");
            result.AppendLine("#define Epsilon 10e-6");
            return result.ToString();
        }

#endif

        protected static T[] ExpandArray<T>(T[] array, int newSize) {
            if (array == null) {
                throw new ArgumentNullException(nameof(array));
            }
            if (newSize < 0) {
                throw new ArgumentOutOfRangeException(nameof(newSize), "New size must be non-negative.");
            }
            if (array.Length == newSize) {
                return array;
            }
            if (array.Length > newSize) {
                throw new ArgumentException($"Array length is greater {newSize}.");
            }
            var newArray = new T[newSize];
            Array.Copy(array, newArray, array.Length);
            return newArray;
        }
        
        public static T[] ExpandArray<T>(IList<T> list, int newSize) {
            if (list == null) {
                throw new ArgumentNullException(nameof(list));
            }
            if (newSize < 0) {
                throw new ArgumentOutOfRangeException(nameof(newSize), "New size must be non-negative.");
            }
            if (list.Count == newSize) {
                return list.ToArray();
            }
            if (list.Count > newSize) {
                throw new ArgumentException($"List count is greater than {newSize}.");
            }
            var newArray = new T[newSize];
            list.CopyTo(newArray, 0);
            return newArray;
        }


        public abstract void SetFloat(string name, float value);
        public abstract void SetFloatArray(string name, float[] values);

        public abstract void SetInt(string name, int value);

        public abstract void SetVector(string name, UnityEngine.Vector4 value);
        public abstract void SetVectorArray(string name, UnityEngine.Vector4[] values);

        public abstract void SetMatrix(string name, UnityEngine.Matrix4x4 value);
        public abstract void SetMatrixArray(string name, UnityEngine.Matrix4x4[] values);

        public abstract void SetTexture(string name, UnityEngine.Texture value);

        public abstract void EnableKeyword(string name);
        public abstract void DisableKeyword(string name);

        public void SetEnumKeyword<T>(string name, T value) where T : Enum {
            var values = Enum.GetValues(typeof(T));
            foreach (var enumValue in values) {
                var enumName = $"{name}_{enumValue}";
                if (enumValue.Equals(value)) {
                    EnableKeyword(enumName);
                } else {
                    DisableKeyword(enumName);
                }
            }
        }

        public void SetEnumKeyword(string name, int value, int min, int max) {
            for (int i = min; i <= max; i++) {
                var enumName = $"{name}_{i}";
                if (i == value) {
                    EnableKeyword(enumName);
                } else {
                    DisableKeyword(enumName);
                }
            }

        }

    }
}
