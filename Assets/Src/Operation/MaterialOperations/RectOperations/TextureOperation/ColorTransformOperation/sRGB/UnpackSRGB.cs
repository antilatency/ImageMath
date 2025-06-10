using UnityEngine;
#nullable enable
namespace ImageMath{
    [FilePath]
    public partial record UnpackSRGB : ColorTransformOperation {
        public UnpackSRGB(Texture texture) : base(texture) { }
        public UnpackSRGB() : base() { }
        public static string GetColorTransform() {
            return @"
float3 x = max(0, inputColor.rgb);
x = x <= 0.04045 ? x / 12.92 : pow((x + 0.055) / 1.055, 2.4);
return float4(x.r, x.g, x.b, inputColor.a);";
        }

        public override float Convert(float x) {
            if (x < 0) return 0;
            return x <= 0.04045f ? x / 12.92f : Mathf.Pow((x + 0.055f) / 1.055f, 2.4f);
        }

        public override Vector4 Convert(Vector4 x) {
            x.x = Convert(x.x);
            x.y = Convert(x.y);
            x.z = Convert(x.z);
            return x;
        }
        
        public override ColorTransformOperation CreateInverse(Texture? texture = null) {
            return new PackSRGB {
                Texture = texture
            };
        }
    }
}