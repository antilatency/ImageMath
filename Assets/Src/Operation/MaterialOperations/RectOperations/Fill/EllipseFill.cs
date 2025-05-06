using UnityEngine;
namespace ImageMath{
    [FilePath]
    public partial record EllipseFill: RectOperation {
        public Vector4 InnerColor = Vector4.one;
        public Vector4 OuterColor = Vector4.zero;
        public Vector2 Center = Vector3.one * 0.5f;
        public Vector2 Radius = Vector3.one * 0.5f;
        
        public EllipseFill(Vector4 innerColor, Vector4? outerColor = null, Vector2? center = null, Vector2? radius = null) {
            InnerColor = innerColor;
            OuterColor = outerColor ?? Vector4.zero;
            Center = center ?? Vector2.one * 0.5f;
            Radius = radius ?? Vector2.one * 0.5f;
        }
        public EllipseFill() {}

        public static string GetFragmentShaderBody() {
            return @"
float2 innerSpace = (input.uv - Center) / Radius;
float r = length(innerSpace);
return lerp(InnerColor, OuterColor, r>=1);";
        }
    }        
}
