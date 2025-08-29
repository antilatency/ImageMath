using UnityEditor;
using UnityEngine;
#nullable enable
namespace ImageMath {
    public class GeneratorSettings : ScriptableObject {
        public bool isDebugMode;

        private const string assetPath = "Assets/ImageMathGeneratorSettings.asset";

        public static GeneratorSettings? Get() {
            var settings = AssetDatabase.LoadAssetAtPath<GeneratorSettings>(assetPath);
            return settings;
        }

        [MenuItem("ImageMath/Create Generator Settings")]
        public static GeneratorSettings Create() {
            var settings = ScriptableObject.CreateInstance<GeneratorSettings>();
            settings.isDebugMode = false;
            AssetDatabase.CreateAsset(settings, assetPath);
            AssetDatabase.SaveAssets();
            return settings;
        }

        public static bool IsDebugMode() {
            var settings = Get();
            return settings?.isDebugMode ?? false;
        }

        public static GeneratorSettings GetOrCreate() {
            var settings = Get();
            if (settings == null) {
                return Create();
            }
            return settings;
        }

        public void Save() {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
}