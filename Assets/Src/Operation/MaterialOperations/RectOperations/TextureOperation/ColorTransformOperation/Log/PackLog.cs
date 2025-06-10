using UnityEngine;
#nullable enable
namespace ImageMath {
    [FilePath]
    public partial record PackLog : ColorTransformOperation {
        public Vector3 WhiteLevel { get; set; } = new Vector3(1, 1, 1);
        public Vector3 BlackLevel { get; set; } = new Vector3(0, 0, 0);
        public Vector3 ExponentScale { get; set; } = new Vector3(1, 1, 1);

        public PackLog(Texture texture) : base(texture) { }
        public PackLog() : base() { }

        public static string GetColorTransform() {
            return @"
float3 range = WhiteLevel - BlackLevel;
float3 exponentScale = ExponentScale * range;

float3 divider = pow(2, exponentScale) - 1;
float3 x = inputColor.rgb * divider;
x = log2(x + 1) / exponentScale;

x = x * range + BlackLevel;

return float4(x.r, x.g, x.b, inputColor.a);";
        }

        public override Vector4 Convert(Vector4 inputColor) {

            Vector3 x = new();
            for (int i = 0; i < 3; i++) {
                float range = WhiteLevel[i] - BlackLevel[i];
                float exponentScale = ExponentScale[i] * range;
                float divider = Mathf.Pow(2, exponentScale) - 1;
                x[i] = inputColor[i] * divider;
                x[i] = Mathf.Log(x[i] + 1, 2) / exponentScale * range + BlackLevel[i];
            }

            return new Vector4(x.x, x.y, x.z, inputColor.w);
        }

        public override ColorTransformOperation CreateInversed(Texture? texture = null) {
            return new UnpackLog {
                Texture = texture,
                WhiteLevel = WhiteLevel,
                BlackLevel = BlackLevel,
                ExponentScale = ExponentScale
            };
        }
    }
}
