using UnityEngine;
namespace ImageMath{
    [FilePath]
    public partial record PackSRGB : ColorTransformOperation{
        public PackSRGB(Texture texture) : base(texture) { }
        public PackSRGB() : base() { }
        public static string GetColorTransform() {
            return @"
float3 x = max(0, inputColor.rgb);
x = x < 0.0031308 ? 12.92 * x : 1.055 * pow(x, 1.0 / 2.4) - 0.055;
return float4(x.r, x.g, x.b, inputColor.a);";
        }
    }
}