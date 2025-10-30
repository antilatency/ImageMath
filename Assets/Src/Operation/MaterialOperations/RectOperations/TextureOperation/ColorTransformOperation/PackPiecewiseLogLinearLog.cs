#nullable enable

using UnityEngine;

namespace ImageMath {

    [FilePath]
    public partial record PackPiecewiseLogLinearLog : ColorTransformOperation {

        public float LeftThreshold { get; set; }
        public float RightThreshold { get; set; }
        public float LeftLogInnerScale { get; set; }
        public float LeftLogInnerOffset { get; set; }
        public float LeftLogOuterScale { get; set; }
        public float LeftLogOuterOffset { get; set; }
        public float LinearScale { get; set; }
        public float LinearOffset { get; set; }
        public float RightLogInnerScale { get; set; }
        public float RightLogInnerOffset { get; set; }
        public float RightLogOuterScale { get; set; }
        public float RightLogOuterOffset { get; set; }

        public PackPiecewiseLogLinearLog(Texture? texture = null) : base(texture) {
        }

        public override ColorTransformOperation CreateInverse(Texture? texture = null) {
            return new UnpackPiecewiseLogLinearLog(texture) {
                LeftThreshold = LeftThreshold,
                RightThreshold = RightThreshold,
                LeftLogInnerScale = LeftLogInnerScale,
                LeftLogInnerOffset = LeftLogInnerOffset,
                LeftLogOuterScale = LeftLogOuterScale,
                LeftLogOuterOffset = LeftLogOuterOffset,
                LinearScale = LinearScale,
                LinearOffset = LinearOffset,
                RightLogInnerScale = RightLogInnerScale,
                RightLogInnerOffset = RightLogInnerOffset,
                RightLogOuterScale = RightLogOuterScale,
                RightLogOuterOffset = RightLogOuterOffset,
            };
        }

        public override float Convert(float x) {
            if (x < LeftThreshold) {
                return LeftLogOuterScale * Mathf.Log(LeftLogInnerScale * x + LeftLogInnerOffset) + LeftLogOuterOffset;
            }

            if (x > RightThreshold) {
                return RightLogOuterScale * Mathf.Log(RightLogInnerScale * x + RightLogInnerOffset) + RightLogOuterOffset;
            }

            return LinearScale * x + LinearOffset;
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
float3 linearPiece = LinearScale * x + LinearOffset;
float3 leftLogPiece = LeftLogOuterScale * log(LeftLogInnerScale * x + LeftLogInnerOffset) + LeftLogOuterOffset;
float3 rightLogPiece = RightLogOuterScale * log(RightLogInnerScale * x + RightLogInnerOffset) + RightLogOuterOffset;
float3 y = (x < LeftThreshold ? leftLogPiece : (x > RightThreshold ? rightLogPiece : linearPiece));
return float4(y, inputColor.a);
";
        }
    }
}
