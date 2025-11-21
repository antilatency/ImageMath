using UnityEngine;

namespace ImageMath {
    [FilePath]
    public partial record TextureRGBMultipliedByMatrix : ColorTransformOperation {
        public Matrix4x4 Matrix { get; set; } = Matrix4x4.identity;
        public TextureRGBMultipliedByMatrix(Texture texture, Matrix4x4 matrix) : base(texture) {
            Matrix = matrix;
        }
        public TextureRGBMultipliedByMatrix() : base() { }
        public static string GetColorTransform() {
            return @"
float alpha = inputColor.a;
inputColor.a = 1.0f;
float4 result = mul(Matrix, inputColor);
result.a = alpha;
return result;
";
        }
    }

}
