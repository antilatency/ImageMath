using UnityEngine;
namespace ImageMath {
    [FilePath]
    public partial record UnpackLog : ColorTransformOperation {
        public Vector3 WhiteLevel { get; set; } = new Vector3(1, 1, 1);
        public Vector3 BlackLevel { get; set; } = new Vector3(0, 0, 0);
        public Vector3 ExponentScale { get; set; } = new Vector3(1, 1, 1);

        public UnpackLog(Texture texture) : base(texture) { }
        public UnpackLog() : base() { }
        public static string GetColorTransform() {
            return @"
float3 range = WhiteLevel - BlackLevel;
float3 x = (inputColor.rgb - BlackLevel) / range;

float3 exponentScale = ExponentScale * range;

x = pow(2, x * exponentScale) - 1;
float3 divider = pow(2, exponentScale) - 1;
x /= divider;

return float4(x.r, x.g, x.b, inputColor.a);";
        }

        public override Vector4 Convert(Vector4 inputColor) {
            /*Vector3 range = WhiteLevel - BlackLevel;
            Vector3 x = (Vector3)inputColor - BlackLevel;
            x.x /= range.x;
            x.y /= range.y;
            x.z /= range.z;
            Vector3 exponentScale = new Vector3(ExponentScale.x * range.x, ExponentScale.y * range.y, ExponentScale.z * range.z);
            x.x = Mathf.Pow(2, x.x * exponentScale.x) - 1;
            x.y = Mathf.Pow(2, x.y * exponentScale.y) - 1;
            x.z = Mathf.Pow(2, x.z * exponentScale.z) - 1;
            Vector3 divider = new Vector3(Mathf.Pow(2, exponentScale.x) - 1, Mathf.Pow(2, exponentScale.y) - 1, Mathf.Pow(2, exponentScale.z) - 1);
            x.x /= divider.x;
            x.y /= divider.y;
            x.z /= divider.z;*/
            Vector3 x = new();
            for (int i = 0; i < 3; i++) {
                float range = WhiteLevel[i] - BlackLevel[i];
                x[i] = (inputColor[i] - BlackLevel[i]) / range;
                float exponentScale = ExponentScale[i] * range;
                x[i] = Mathf.Pow(2, x[i] * exponentScale) - 1;
                float divider = Mathf.Pow(2, exponentScale) - 1;
                x[i] /= divider;
            }
            return new Vector4(x.x, x.y, x.z, inputColor.w);
        }
    }
}