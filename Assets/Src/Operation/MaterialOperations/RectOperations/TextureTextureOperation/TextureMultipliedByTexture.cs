using UnityEngine;

namespace ImageMath {
    [FilePath]
    public partial record TextureMultipliedByTexture : TextureTextureColorOperation {

        public TextureMultipliedByTexture() : base() { }

        #if UNITY_EDITOR
        public static string GetColorTransform() {
            return @"return inputColorA * inputColorB;";
        }
        #endif
    }
}
