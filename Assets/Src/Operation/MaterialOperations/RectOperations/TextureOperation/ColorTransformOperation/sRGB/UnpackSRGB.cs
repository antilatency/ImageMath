using UnityEngine;
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
        
        public override Vector4 Convert(Vector4 inputColor) {
            Vector3 x = new Vector3(Mathf.Max(0, inputColor.x), Mathf.Max(0, inputColor.y), Mathf.Max(0, inputColor.z));
            x.x = x.x <= 0.04045f ? x.x / 12.92f : Mathf.Pow((x.x + 0.055f) / 1.055f, 2.4f);
            x.y = x.y <= 0.04045f ? x.y / 12.92f : Mathf.Pow((x.y + 0.055f) / 1.055f, 2.4f);
            x.z = x.z <= 0.04045f ? x.z / 12.92f : Mathf.Pow((x.z + 0.055f) / 1.055f, 2.4f);
            return new Vector4(x.x, x.y, x.z, inputColor.w);
        }
    }
}