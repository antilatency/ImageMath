using UnityEngine;

namespace ImageMath {
    public static class TypeCastExtensions {
        public static Vector3 ToVector3(this Color color) {
            return new Vector3(color.r, color.g, color.b);
        }
        public static Vector4 ToVector4(this Color color) {
            return new Vector4(color.r, color.g, color.b, color.a);
        }

    }



}
