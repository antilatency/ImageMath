using UnityEngine;
#nullable enable
namespace ImageMath{
    [FilePath]
    public partial record PackSRGB : ColorTransformOperation {
        public PackSRGB(Texture texture) : base(texture) { }
        public PackSRGB() : base() { }
        public static string GetColorTransform() {
            return @"
float3 x = max(0, inputColor.rgb);
x = x < 0.0031308 ? 12.92 * x : 1.055 * pow(x, 1.0 / 2.4) - 0.055;
return float4(x.r, x.g, x.b, inputColor.a);";
        }

        public override Vector4 Convert(Vector4 inputColor) {
            Vector3 x = new Vector3(Mathf.Max(0, inputColor.x), Mathf.Max(0, inputColor.y), Mathf.Max(0, inputColor.z));
            x.x = x.x < 0.0031308f ? 12.92f * x.x : 1.055f * Mathf.Pow(x.x, 1.0f / 2.4f) - 0.055f;
            x.y = x.y < 0.0031308f ? 12.92f * x.y : 1.055f * Mathf.Pow(x.y, 1.0f / 2.4f) - 0.055f;
            x.z = x.z < 0.0031308f ? 12.92f * x.z : 1.055f * Mathf.Pow(x.z, 1.0f / 2.4f) - 0.055f;
            return new Vector4(x.x, x.y, x.z, inputColor.w);
        }
        
        public override ColorTransformOperation CreateInverse(Texture? texture = null) {
            return new UnpackSRGB {
                Texture = texture
            };
        }
    }
}