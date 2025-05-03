using UnityEngine;
namespace ImageMath{
    public partial record UnpackSRGB : ColorTransformOperation{
        public UnpackSRGB(Texture texture) : base(texture) { }
        public UnpackSRGB() : base() { }
        public static string GetColorTransform() {
            return @"
    float3 x = max(0, inputColor.rgb);
    x = x <= 0.04045 ? x / 12.92 : pow((x + 0.055) / 1.055, 2.4);
    return float4(x.r, x.g, x.b, inputColor.a);";
        }
    }
}