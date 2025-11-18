#nullable enable

using UnityEngine;

namespace ImageMath {

    [FilePath]
    public partial record UnpackPiecewiseLinearPow : ColorTransformOperation {

        public float Threshold;
        public float LinearScale;
        public float LinearOffset;
        public float PowInnerScale;
        public float PowInnerOffset;
        public float PowExponent;
        public float PowOuterScale;
        public float PowOuterOffset;

        public float InvThreshold => (float)(LinearScale * Threshold + LinearOffset);
        public float InvLinearScale => (float)(1 / LinearScale);
        public float InvLinearOffset => (float)(-LinearOffset / LinearScale);
        public float InvPowInnerScale => (float)(1 / PowOuterScale);
        public float InvPowInnerOffset => (float)(-PowOuterOffset / PowOuterScale);
        public float InvPowExponent => (float)(1.0 / PowExponent);
        public float InvPowOuterScale => (float)(1 / PowInnerScale);
        public float InvPowOuterOffset => (float)(-PowInnerOffset / PowInnerScale);

        public UnpackPiecewiseLinearPow(Texture? texture = null) : base(texture) {
        }

        public override ColorTransformOperation CreateInverse(Texture? texture = null) {
            return new PackPiecewiseLinearPow() {
                Threshold = Threshold,
                LinearScale = LinearScale,
                LinearOffset = LinearOffset,
                PowInnerScale = PowInnerScale,
                PowInnerOffset = PowInnerOffset,
                PowExponent = PowExponent,
                PowOuterScale = PowOuterScale,
                PowOuterOffset = PowOuterOffset,
            };
        }

        public override float Convert(float y) {
            if (y < InvThreshold) {
                return InvLinearScale * y + InvLinearOffset;
            }
            else {
                return InvPowOuterScale * Mathf.Pow(InvPowInnerScale * y + InvPowInnerOffset, InvPowExponent) + InvPowOuterOffset;
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
float3 powPiece = InvPowOuterScale * pow(InvPowInnerScale * y + InvPowInnerOffset, InvPowExponent) + InvPowOuterOffset;
float3 x = (y < InvThreshold ? linearPiece : powPiece);
return float4(x, inputColor.a);
";
        }
    }
}
