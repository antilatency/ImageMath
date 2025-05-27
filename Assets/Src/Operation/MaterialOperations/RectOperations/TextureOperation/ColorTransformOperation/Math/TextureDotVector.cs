using UnityEngine;
namespace ImageMath {
    [FilePath]
    public partial record TextureDotVector : ColorTransformOperation {
        public Vector4 Vector { get; set; } = new Vector4(1, 1, 1, 1);
        public TextureDotVector(Texture texture, Vector4 vector) : base(texture) {
            Vector = vector;
        }
        public TextureDotVector() : base() { }
        public static string GetColorTransform() {
            return @"
return dot(inputColor,Vector);";
        }
    }
}
