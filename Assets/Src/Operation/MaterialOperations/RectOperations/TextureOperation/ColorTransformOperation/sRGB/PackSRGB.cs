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

        public override float Convert(float x) {
            if (x < 0) return 0;
            return x < 0.0031308f ? 12.92f * x : 1.055f * Mathf.Pow(x, 1.0f / 2.4f) - 0.055f;
        }

        public override Vector4 Convert(Vector4 x) {
            x.x = Convert(x.x);
            x.y = Convert(x.y);
            x.z = Convert(x.z);
            return x;
        }

        public override ColorTransformOperation CreateInverse(Texture? texture = null) {
            return new UnpackSRGB {
                Texture = texture
            };
        }
    }
}