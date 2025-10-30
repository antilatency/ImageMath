#nullable enable

using System;

using UnityEngine;

namespace ImageMath {

    [FilePath]
    public partial record UnpackPiecewiseLogLog : ColorTransformOperation {

        public float Threshold;
        public float LeftLogInnerScale;
        public float LeftLogInnerOffset;
        public float LeftLogOuterScale;
        public float LeftLogOuterOffset;
        public float RightLogInnerScale;
        public float RightLogInnerOffset;
        public float RightLogOuterScale;
        public float RightLogOuterOffset;

        public float InvThreshold => (float)(LeftLogOuterScale * Math.Log(LeftLogInnerScale * Threshold + LeftLogInnerOffset) + LeftLogOuterOffset);
        public float InvLeftExpInnerScale => (float)(1 / LeftLogOuterScale);
        public float InvLeftExpInnerOffset => (float)(-LeftLogOuterOffset / LeftLogOuterScale);
        public float InvLeftExpOuterScale => (float)(1 / LeftLogInnerScale);
        public float InvLeftExpOuterOffset => (float)(-LeftLogInnerOffset / LeftLogInnerScale);
        public float InvRightExpInnerScale => (float)(1 / RightLogOuterScale);
        public float InvRightExpInnerOffset => (float)(-RightLogOuterOffset / RightLogOuterScale);
        public float InvRightExpOuterScale => (float)(1 / RightLogInnerScale);
        public float InvRightExpOuterOffset => (float)(-RightLogInnerOffset / RightLogInnerScale);

        public UnpackPiecewiseLogLog(Texture? texture = null) : base(texture) {
        }

        public override ColorTransformOperation CreateInverse(Texture? texture = null) {
            return new PackPiecewiseLogLog() {
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

        public override float Convert(float y) {
            if (y < InvThreshold) {
                return InvLeftExpOuterScale * Mathf.Exp(InvLeftExpInnerScale * y + InvLeftExpInnerOffset) + InvLeftExpOuterOffset;
            }
            else {
                return InvRightExpOuterScale * Mathf.Exp(InvRightExpInnerScale * y + InvRightExpInnerOffset) + InvRightExpOuterOffset;
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
float3 y = inputColor.rgb;
float3 leftExpPiece = InvLeftExpOuterScale * exp(InvLeftExpInnerScale * y + InvLeftExpInnerOffset) + InvLeftExpOuterOffset;
float3 rightExpPiece = InvRightExpOuterScale * exp(InvRightExpInnerScale * y + InvRightExpInnerOffset) + InvRightExpOuterOffset;
float3 x = (y < InvThreshold ? leftExpPiece : rightExpPiece);
return float4(x, inputColor.a);
";
        }
    }
}
