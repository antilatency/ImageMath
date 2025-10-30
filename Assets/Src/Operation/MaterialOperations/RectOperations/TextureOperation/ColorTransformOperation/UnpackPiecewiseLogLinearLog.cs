#nullable enable

using UnityEngine;

namespace ImageMath {

    [FilePath]
    public partial record UnpackPiecewiseLogLinearLog : ColorTransformOperation {

        public float LeftThreshold;
        public float RightThreshold;
        public float LeftLogInnerScale;
        public float LeftLogInnerOffset;
        public float LeftLogOuterScale;
        public float LeftLogOuterOffset;
        public float LinearScale;
        public float LinearOffset;
        public float RightLogInnerScale;
        public float RightLogInnerOffset;
        public float RightLogOuterScale;
        public float RightLogOuterOffset;

        public float InvLeftThreshold => (float)(LinearScale * LeftThreshold + LinearOffset);
        public float InvRightThreshold => (float)(LinearScale * RightThreshold + LinearOffset);
        public float InvLinearScale => (float)(1 / LinearScale);
        public float InvLinearOffset => (float)(-LinearOffset / LinearScale);
        public float InvLeftExpInnerScale => (float)(1 / LeftLogOuterScale);
        public float InvLeftExpInnerOffset => (float)(-LeftLogOuterOffset / LeftLogOuterScale);
        public float InvLeftExpOuterScale => (float)(1 / LeftLogInnerScale);
        public float InvLeftExpOuterOffset => (float)(-LeftLogInnerOffset / LeftLogInnerScale);
        public float InvRightExpInnerScale => (float)(1 / RightLogOuterScale);
        public float InvRightExpInnerOffset => (float)(-RightLogOuterOffset / RightLogOuterScale);
        public float InvRightExpOuterScale => (float)(1 / RightLogInnerScale);
        public float InvRightExpOuterOffset => (float)(-RightLogInnerOffset / RightLogInnerScale);

        public UnpackPiecewiseLogLinearLog(Texture? texture = null) : base(texture) {
        }

        public override ColorTransformOperation CreateInverse(Texture? texture = null) {
            return new PackPiecewiseLogLinearLog() {
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

        public override float Convert(float y) {
            if (y < InvLeftThreshold) {
                return InvLeftExpOuterScale * Mathf.Exp(InvLeftExpInnerScale * y + InvLeftExpInnerOffset) + InvLeftExpOuterOffset;
            }

            if (y > InvRightThreshold) {
                return InvRightExpOuterScale * Mathf.Exp(InvRightExpInnerScale * y + InvRightExpInnerOffset) + InvRightExpOuterOffset;
            }

            return InvLinearScale * y + InvLinearOffset;
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
float3 y = inputColor.rgb;
float3 linearPiece = InvLinearScale * y + InvLinearOffset;
float3 leftExpPiece = InvLeftExpOuterScale * exp(InvLeftExpInnerScale * y + InvLeftExpInnerOffset) + InvLeftExpOuterOffset;
float3 rightExpPiece = InvRightExpOuterScale * exp(InvRightExpInnerScale * y + InvRightExpInnerOffset) + InvRightExpOuterOffset;
float3 x = (y < InvLeftThreshold ? leftExpPiece : (y > InvRightThreshold ? rightExpPiece : linearPiece));
return float4(x, inputColor.a);
";
        }
    }
}
