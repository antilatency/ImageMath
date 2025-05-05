using UnityEngine;
namespace ImageMath{
    [FilePath]
    public partial record ColorFill: FragmentOperation {
        public Vector4 Color;
        public ColorFill(Vector4 color) {
            Color = color;
        }
        public ColorFill() {
            Color = new Vector4(0, 0, 0, 1);
        }

        public static string GetFragmentShaderBody() {
            return "return Color;";
        }
    }
}
