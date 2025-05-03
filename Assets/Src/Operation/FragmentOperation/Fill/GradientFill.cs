using UnityEngine;
namespace ImageMath{
    public partial record GradientFill: FragmentOperation {
        public Vector4 ColorA = new Vector4(0, 0, 0, 1);
        public Vector4 ColorB = new Vector4(1, 1, 1, 1);
        public Vector2 PointA = new Vector2(0, 0);
        public Vector2 PointB = new Vector2(1, 0);
        public GradientFill(Vector4 colorA, Vector4 colorB, Vector2? pointA, Vector2? pointB) {
            ColorA = colorA;
            ColorB = colorB;
            PointA = pointA ?? new Vector2(0, 0);
            PointB = pointB ?? new Vector2(1, 0);
        }
        public GradientFill() :base(){}

        public static string GetFragmentShaderBody() {
                return @"
float2 dir = normalize(PointB - PointA);
float t = dot(input.uv - PointA, dir) / dot(PointB - PointA, dir);
t = saturate(t);
return lerp(ColorA, ColorB, t);";
        }
    }
}
