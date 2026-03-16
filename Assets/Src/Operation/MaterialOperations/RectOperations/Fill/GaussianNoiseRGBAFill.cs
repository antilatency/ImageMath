using UnityEngine;

namespace ImageMath {
    [FilePath]
    public partial record GaussianNoiseRGBAFill : RectOperation {
        public Vector4 Amplitude { get; set; }


        public GaussianNoiseRGBAFill(Vector4 amplitude) {
            Amplitude = amplitude;
        }

        public GaussianNoiseRGBAFill(float amplitude) {
            Amplitude = new Vector4(amplitude, amplitude, amplitude, amplitude);
        }

        public GaussianNoiseRGBAFill() {
            Amplitude = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
        }

#if UNITY_EDITOR

        public static string GetCustomCode() => IncludeOrEmbed("Noise.cginc");

        public static string GetFragmentShaderBody() {
            return "return Amplitude * GaussianNoiseRGBA(input.uv);";
        }        
#endif
    }
}
