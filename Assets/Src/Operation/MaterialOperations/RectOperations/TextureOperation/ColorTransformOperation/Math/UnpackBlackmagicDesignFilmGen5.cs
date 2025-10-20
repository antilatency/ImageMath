

using UnityEngine;
#nullable enable

#if UNITY_EDITOR
#endif

namespace ImageMath {
    [FilePath]
    public partial record UnpackBlackmagicDesignFilmGen5 : ColorTransformOperation {
            // Blackmagic Design Film Gen 5 constants (verified)
        private const float A = 8.283605932402494f;
        private const float B = 0.09246575342465753f;
        private const float C = 0.005494072432257808f;
        private const float D = 0.08692876065491224f;
        private const float E = 0.5300133392291939f;
        private const float LinCut = 0.005f;
        private const float LogCut = A*LinCut + B;
        
        
        public float Multiplier { get; set; } = 1.0f;

        public UnpackBlackmagicDesignFilmGen5(Texture texture) : base(texture) { }

        public static string GetColorTransform() {
            return @$"
static const float iA = {1.0/A};
static const float B = {B};
static const float C = {C};
static const float iD = {1.0/D};
static const float E = {E};
static const float Breakpoint = {LogCut};

float3 x = inputColor.rgb;
float3 linearPart = (x - B) * iA;
float3 expPart = exp((x - E) * iD) - C;
x = (x <= Breakpoint) ? linearPart : expPart;
x *= Multiplier;

return float4(x, inputColor.a);
";
        }


        public override float Convert(float x) {
            float raw;
            if (x <= LogCut)
                raw = (x - B) / A;
            else
                raw = Mathf.Exp((x - E) / D) - C;
            return raw * Multiplier;
        }

        public override Vector4 Convert(Vector4 x) {
            x.x = Convert(x.x);
            x.y = Convert(x.y);
            x.z = Convert(x.z);
            return x;
        }
    }



}
