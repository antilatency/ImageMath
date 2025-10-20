using UnityEngine;
#nullable enable
namespace ImageMath {
    [FilePath]
    public partial record AbsDiffOperation : TextureTextureColorOperation {
        public Vector4 Multiplier { get; set; } = new Vector4(1, 1, 1, 1);
        public AbsDiffOperation(Texture textureA, Texture textureB, Vector4 multiplier) : base(textureA, textureB) {
            Multiplier = multiplier;
        }
        public AbsDiffOperation(Texture textureA, Texture textureB, float multiplier = 1) : base(textureA, textureB) {
            Multiplier = new Vector4(multiplier, multiplier, multiplier, multiplier);
        }
        public AbsDiffOperation() : base() { }

        public static string GetColorTransform() {
            return @"
float4 absDiff = abs(inputColorA - inputColorB) * Multiplier;
return absDiff;
";
        }
    }

}
