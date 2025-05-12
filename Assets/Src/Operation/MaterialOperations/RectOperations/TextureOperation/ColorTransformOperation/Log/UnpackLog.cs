using UnityEngine;
namespace ImageMath{
    [FilePath]
    public partial record UnpackLog : ColorTransformOperation{
        public Vector3 WhiteLevel {get; set;} = new Vector3(1, 1, 1);
        public Vector3 BlackLevel {get; set;} = new Vector3(0, 0, 0);
        public Vector3 ExponentScale {get; set;} = new Vector3(1, 1, 1);

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
    }
}