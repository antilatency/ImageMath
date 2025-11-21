#nullable enable

using UnityEngine;

namespace ImageMath {
    [FilePath]
    public partial record PackYUV : ColorTransformOperation {
        [MulticompileOptions]
        public YUV.Standard Standard { get; set; } = YUV.Standard.BT709;
        public Matrix4x4 Matrix => YUV.GetRGBToYUVMatrix(Standard);
        //public bool FullRange { get; set; } 

        public PackYUV(Texture? texture = null) : base(texture) {
        }

        public override ColorTransformOperation CreateInverse(Texture? texture = null) {
            throw new System.NotImplementedException();
            /*return new UnpackYUV(texture) {
                Standard = Standard,
                FullRange = FullRange,
            };*/
        }

        public override Vector4 Convert(Vector4 x) {
            float alpha = x.w;
            x.w = 1.0f;
            Matrix4x4 rgbToYuv = YUV.GetRGBToYUVMatrix(Standard);
            x = rgbToYuv * x;
            x.w = alpha;
            return x;
        }

        public static string GetColorTransform() {
            return @"
float3 x = inputColor.rgb;
float3 y = mul(Matrix, float4(x, 1.0)).rgb;
return float4(y, inputColor.a);
";
        }

    }
}
