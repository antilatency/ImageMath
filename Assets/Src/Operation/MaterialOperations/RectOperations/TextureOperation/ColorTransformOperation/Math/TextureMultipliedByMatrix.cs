using UnityEngine;

namespace ImageMath {
    [FilePath]
    public partial record TextureMultipliedByMatrix : ColorTransformOperation {
        public Matrix4x4 Matrix { get; set; } = Matrix4x4.identity;
        public TextureMultipliedByMatrix(Texture texture, Matrix4x4 matrix) : base(texture) {
            Matrix = matrix;
        }
        public TextureMultipliedByMatrix() : base() { }
        public static string GetColorTransform() {
            return @"
return mul(Matrix, inputColor);
";
        }
    }

}
