using UnityEngine;

#nullable enable

namespace ImageMath.ScriptableObjects {
    public class ScriptableWrapper<T> : ScriptableObject {
        public T Value;
    }

    public static class ScriptableWrapperExtensions {
#if UNITY_EDITOR
        public static void SaveAsScriptableWrapper<T>(T value, string path) {
            var wrapper = ScriptableObject.CreateInstance<ScriptableWrapper<T>>();
            wrapper.Value = value;
            UnityEditor.AssetDatabase.CreateAsset(wrapper, path);
            UnityEditor.AssetDatabase.SaveAssets();
        }
#endif
    }

    public class ScriptableMatrix4x4 : ScriptableWrapper<Matrix4x4> { }
    

}