#nullable enable

using UnityEngine;

namespace ImageMath {

    [FilePath]
    public partial record PackPiecewiseLogLog : ColorTransformOperation {

        public float Threshold { get; set; }
        public float LeftLogInnerScale { get; set; }
        public float LeftLogInnerOffset { get; set; }
        public float LeftLogOuterScale { get; set; }
        public float LeftLogOuterOffset { get; set; }
        public float RightLogInnerScale { get; set; }
        public float RightLogInnerOffset { get; set; }
        public float RightLogOuterScale { get; set; }
        public float RightLogOuterOffset { get; set; }

        public PackPiecewiseLogLog(Texture? texture = null) : base(texture) {
        }

        public override ColorTransformOperation CreateInverse(Texture? texture = null) {
            return new UnpackPiecewiseLogLog(texture) {
                Threshold = Threshold,
                LeftLogInnerScale = LeftLogInnerScale,
                LeftLogInnerOffset = LeftLogInnerOffset,
                LeftLogOuterScale = LeftLogOuterScale,
                LeftLogOuterOffset = LeftLogOuterOffset,
                RightLogInnerScale = RightLogInnerScale,
                RightLogInnerOffset = RightLogInnerOffset,
                RightLogOuterScale = RightLogOuterScale,
                RightLogOuterOffset = RightLogOuterOffset,
            };
        }

        public override float Convert(float x) {
            if (x < Threshold) {
                return LeftLogOuterScale * Mathf.Log(LeftLogInnerScale * x + LeftLogInnerOffset) + LeftLogOuterOffset;
            } else {
                return RightLogOuterScale * Mathf.Log(RightLogInnerScale * x + RightLogInnerOffset) + RightLogOuterOffset;
            }
        }

        public override Vector4 Convert(Vector4 x) {
            x.x = Convert(x.x);
            x.y = Convert(x.y);
            x.z = Convert(x.z);
            return x;
        }

        public static string GetColorTransform() {
            // TODO: indent somehow.
            return @$"
float3 x = inputColor.rgb;
float3 leftLogPiece = LeftLogOuterScale * log(LeftLogInnerScale * x + LeftLogInnerOffset) + LeftLogOuterOffset;
float3 rightLogPiece = RightLogOuterScale * log(RightLogInnerScale * x + RightLogInnerOffset) + RightLogOuterOffset;
float3 y = (x < Threshold ? leftLogPiece : rightLogPiece);
return float4(y, inputColor.a);
";
        }
    }
}
