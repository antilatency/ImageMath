#nullable enable

using UnityEngine;

namespace ImageMath {

    [FilePath]
    public partial record PackPiecewiseLinearPow : ColorTransformOperation {
        public float Threshold { get; set; }
        public float LinearScale { get; set; }
        public float LinearOffset { get; set; }
        public float PowInnerScale { get; set; }
        public float PowInnerOffset { get; set; }
        public float PowExponent { get; set; }
        public float PowOuterScale { get; set; }
        public float PowOuterOffset { get; set; }

        public PackPiecewiseLinearPow(Texture? texture = null) : base(texture) {
        }

        public override ColorTransformOperation CreateInverse(Texture? texture = null) {
            return new UnpackPiecewiseLinearPow(texture) {
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

        public override float Convert(float x) {
            if (x < Threshold) {
                return LinearScale * x + LinearOffset;
            }
            else {
                return PowOuterScale * Mathf.Pow(PowInnerScale * x + PowInnerOffset, PowExponent) + PowOuterOffset;
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
float3 linearPiece = LinearScale * x + LinearOffset;
float3 powPiece = PowOuterScale * pow(PowInnerScale * x + PowInnerOffset, PowExponent) + PowOuterOffset;
float3 y = (x < Threshold ? linearPiece : powPiece);
return float4(y, inputColor.a);
";
        }
    }
}
