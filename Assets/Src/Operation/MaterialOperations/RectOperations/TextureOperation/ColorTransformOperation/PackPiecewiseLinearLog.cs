#nullable enable

using UnityEngine;

namespace ImageMath {

    [FilePath]
    public partial record PackPiecewiseLinearLog : ColorTransformOperation {
        public float Threshold { get; set; }
        public float LinearScale { get; set; }
        public float LinearOffset { get; set; }
        public float LogInnerScale { get; set; }
        public float LogInnerOffset { get; set; }
        public float LogOuterScale { get; set; }
        public float LogOuterOffset { get; set; }

        public PackPiecewiseLinearLog(Texture? texture = null) : base(texture) {
        }

        public override ColorTransformOperation CreateInverse(Texture? texture = null) {
            return new UnpackPiecewiseLinearLog(texture) {
                Threshold = Threshold,
                LinearScale = LinearScale,
                LinearOffset = LinearOffset,
                LogInnerScale = LogInnerScale,
                LogInnerOffset = LogInnerOffset,
                LogOuterScale = LogOuterScale,
                LogOuterOffset = LogOuterOffset,
            };
        }

        public override float Convert(float x) {
            if (x < Threshold) {
                return LinearScale * x + LinearOffset;
            }
            else {
                return LogOuterScale * Mathf.Log(LogInnerScale * x + LogInnerOffset) + LogOuterOffset;
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
float3 logPiece = LogOuterScale * log(LogInnerScale * x + LogInnerOffset) + LogOuterOffset;
float3 y = (x < Threshold ? linearPiece : logPiece);
return float4(y, inputColor.a);
";
        }
    }
}
