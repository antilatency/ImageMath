using UnityEngine;
namespace ImageMath{
    [FilePath]
    public partial record LineFill: RectOperation {
        public Vector4 Color {get; set;} = new Vector4(1, 1, 1, 1);
        public float LineWidth {get; set;} = 0.1f;
        public float LineSoftness {get; set;} = 0.5f;
        public Vector2 PointA {get; set;} = new Vector2(0, 0);
        public Vector2 PointB {get; set;} = new Vector2(1, 0);
        public LineFill(Vector4 color, Vector2 pointA, Vector2 pointB) {
            Color = color;
            PointA = pointA;
            PointB = pointB;
        }
        public LineFill() :base(){}
#if UNITY_EDITOR
        public static string GetFragmentShaderBody() => IncludeOrEmbed("LineFill.FragmentShaderBody.cginc");
#endif
    }
}
