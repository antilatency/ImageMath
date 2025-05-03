using UnityEngine;
namespace ImageMath{
    public partial record TextureMultipliedByVector : ColorTransformOperation{
        public Vector4 Multiplier = new Vector4(1, 1, 1, 1);
        public TextureMultipliedByVector(Texture texture, Vector4 multiplier) : base(texture) {
            Multiplier = multiplier;
        }
        public TextureMultipliedByVector(Texture texture, float multiplierRGB, float multiplierA = 1) : base(texture) {
            Multiplier = new Vector4(multiplierRGB, multiplierRGB, multiplierRGB, multiplierA);
        }
        public TextureMultipliedByVector() : base() { }
        public static string GetColorTransform() {
            return @"
return inputColor * Multiplier;";
        }
    }
}