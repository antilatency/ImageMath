namespace ImageMath {
    [FilePath]
    public partial record GaussianNoiseFill : RectOperation {
        public float Amplitude { get; set; }
        public GaussianNoiseFill(float amplitude) {
            Amplitude = amplitude;
        }
        public GaussianNoiseFill() {
            Amplitude = 1.0f;
        }

#if UNITY_EDITOR

        public static string GetCustomCode() => IncludeOrEmbed("Noise.cginc");

        public static string GetFragmentShaderBody() {
            return "return Amplitude * GaussianNoise(input.uv);";
        }
#endif
    }
}
