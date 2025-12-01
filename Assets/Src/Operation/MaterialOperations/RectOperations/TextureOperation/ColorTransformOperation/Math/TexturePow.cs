using UnityEngine;
namespace ImageMath {
    [FilePath]
    public partial record TexturePow : ColorTransformOperation {
        public Vector4 Exponent { get; set; } = new Vector4(1, 1, 1, 1);
        public TexturePow(Texture texture, Vector4 exponent) : base(texture) {
            Exponent = exponent;
        }
        public TexturePow(Texture texture, float exponentRGB, float exponentA = 1) : base(texture) {
            Exponent = new Vector4(exponentRGB, exponentRGB, exponentRGB, exponentA);
        }

        public TexturePow() : base() { }
        public static string GetColorTransform() {
            return @"
return pow(inputColor, Exponent);";
        }
    }
}
