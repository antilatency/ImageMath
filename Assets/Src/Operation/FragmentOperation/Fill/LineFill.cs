using UnityEngine;
namespace ImageMath{
    public partial record LineFill: FragmentOperation {
        public Vector4 Color = new Vector4(1, 1, 1, 1);
        public float LineWidth = 0.1f;
        public float LineSoftness = 0.5f;
        public Vector2 PointA = new Vector2(0, 0);
        public Vector2 PointB = new Vector2(1, 0);
        public LineFill(Vector4 color, Vector2 pointA, Vector2 pointB) {
            Color = color;
            PointA = pointA;
            PointB = pointB;
        }
        public LineFill() :base(){}

        public static string GetFragmentShaderBody() => Include("LineFill.FragmentShaderBody.cginc");
    }
}
