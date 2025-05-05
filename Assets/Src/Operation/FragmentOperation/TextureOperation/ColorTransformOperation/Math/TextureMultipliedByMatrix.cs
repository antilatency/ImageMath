using UnityEngine;
namespace ImageMath{
    [FilePath]
    public partial record TextureMultipliedByMatrix : ColorTransformOperation{
        public Matrix4x4 Multiplier = Matrix4x4.identity;
        public TextureMultipliedByMatrix(Texture texture, Matrix4x4 multiplier) : base(texture) {
            Multiplier = multiplier;
        }
        public TextureMultipliedByMatrix() : base() { }
        public static string GetColorTransform() {
            return @"
return mul(Multiplier, inputColor);";
        }
    }


}