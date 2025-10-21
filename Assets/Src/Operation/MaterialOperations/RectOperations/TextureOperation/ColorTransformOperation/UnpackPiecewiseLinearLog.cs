#nullable enable

using UnityEngine;

namespace ImageMath {

    [FilePath]
    public partial record UnpackPiecewiseLinearLog : ColorTransformOperation {

        public float Threshold;
        public float LinearScale;
        public float LinearOffset;
        public float LogInnerScale;
        public float LogInnerOffset;
        public float LogOuterScale;
        public float LogOuterOffset;

        public float InvThreshold => (float)(LinearScale * Threshold + LinearOffset);
        public float InvLinearScale => (float)(1 / LinearScale);
        public float InvLinearOffset => (float)(-LinearOffset / LinearScale);
        public float InvExpInnerScale => (float)(1 / LogOuterScale);
        public float InvExpInnerOffset => (float)(-LogOuterOffset / LogOuterScale);
        public float InvExpOuterScale => (float)(1 / LogInnerScale);
        public float InvExpOuterOffset => (float)(-LogInnerOffset / LogInnerScale);

        public UnpackPiecewiseLinearLog(Texture? texture = null) : base(texture) {
        }

        public override ColorTransformOperation CreateInverse(Texture? texture = null) {
            return new PackPiecewiseLinearLog() {
                Threshold = Threshold,
                LinearScale = LinearScale,
                LinearOffset = LinearOffset,
                LogInnerScale = LogInnerScale,
                LogInnerOffset = LogInnerOffset,
                LogOuterScale = LogOuterScale,
                LogOuterOffset = LogOuterOffset,
            };
        }

        public override float Convert(float y) {
            if (y < InvThreshold) {
                return InvLinearScale * y + InvLinearOffset;
            }
            else {
                return InvExpOuterScale * Mathf.Exp(InvExpInnerScale * y + InvExpInnerOffset) + InvExpOuterOffset;
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
float3 linearPiece = InvLinearScale * y + InvLinearOffset;
float3 expPiece = InvExpOuterScale * exp(InvExpInnerScale * y + InvExpInnerOffset) + InvExpOuterOffset;
float3 x = (y < InvThreshold ? linearPiece : expPiece);
return float4(x, inputColor.a);
";
        }
    }
}
