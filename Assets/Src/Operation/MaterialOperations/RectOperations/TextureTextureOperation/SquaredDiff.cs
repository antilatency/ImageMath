using UnityEngine;
#nullable enable
namespace ImageMath {
    [FilePath]
    public partial record SquaredDiff : TextureTextureColorOperation {
        public Vector4 Multiplier { get; set; } = new Vector4(1, 1, 1, 1);
        public SquaredDiff(Texture textureA, Texture textureB, Vector4 multiplier) : base(textureA, textureB) {
            Multiplier = multiplier;
        }
        public SquaredDiff(Texture textureA, Texture textureB, float multiplier = 1) : base(textureA, textureB) {
            Multiplier = new Vector4(multiplier, multiplier, multiplier, multiplier);
        }
        public SquaredDiff() : base() { }

        public static string GetColorTransform() {
            return @"
float4 diff = inputColorA - inputColorB;
float4 squaredDiff = diff * diff * Multiplier;
return squaredDiff;
";
        }
    }
}
