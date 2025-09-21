#if UNITY_EDITOR

using System.IO;
using UnityEditor;
#nullable enable

namespace ImageMath {
    public static class CgincEditorUtils {
        [MenuItem("Assets/Create/Shader/CG Include File")]
        public static void CreateCgincFile() {
            string path = GetSelectedPathOrFallback("NewInclude.cginc");
            string filePath = AssetDatabase.GenerateUniqueAssetPath(path);


            File.WriteAllText(filePath, "");
            AssetDatabase.Refresh();

            // Automatically select and ping the new file
            var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(filePath);
            Selection.activeObject = asset;
            EditorGUIUtility.PingObject(asset);
        }

        private static string GetSelectedPathOrFallback(string newFileName) {
            string path = Path.Combine("Assets", newFileName);
            var selection = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);

            foreach (var obj in selection) {
                path = AssetDatabase.GetAssetPath(obj);

                if (!string.IsNullOrEmpty(path)) {
                    if (File.Exists(path)) {
                        return Path.ChangeExtension(path, Path.GetExtension(newFileName));
                    } else {
                        return Path.Combine(path, newFileName);
                    }
                }
            }
            return path;
        }

    }
}
#endif