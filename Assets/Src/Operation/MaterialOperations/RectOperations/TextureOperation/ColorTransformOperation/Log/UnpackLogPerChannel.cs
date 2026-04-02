using UnityEngine;
#nullable enable
namespace ImageMath {
    /// <summary>
    /// Per-channel log linearization: converts log-encoded input to linear light.
    /// Each RGB channel has its own BlackLevel, WhiteLevel and ExponentScale.
    ///
    /// Formula per channel:
    ///   x = (x - BlackLevel) / (WhiteLevel - BlackLevel)          // normalize to [0,1]
    ///   exponent = ExponentScale * (WhiteLevel - BlackLevel)
    ///   x = (2^(x * exponent) - 1) / (2^exponent - 1)            // remove log curve
    /// </summary>
    [FilePath]
    public partial record UnpackLogPerChannel : TextureOperation {
        public Vector3 WhiteLevel { get; set; } = Vector3.one * 0.5f;
        public Vector3 BlackLevel { get; set; } = Vector3.zero;
        public Vector3 ExponentScale { get; set; } = Vector3.one * 9f;

        public UnpackLogPerChannel(Texture? texture = null) : base(texture) { }

#if UNITY_EDITOR
        public static string GetFragmentShaderBody() => IncludeOrEmbed("UnpackLogPerChannel.FragmentShaderBody.cginc");
#endif

        public Vector3 Convert(Vector3 x) {
            x.x = ConvertChannel(x.x, BlackLevel.x, WhiteLevel.x, ExponentScale.x);
            x.y = ConvertChannel(x.y, BlackLevel.y, WhiteLevel.y, ExponentScale.y);
            x.z = ConvertChannel(x.z, BlackLevel.z, WhiteLevel.z, ExponentScale.z);
            return x;
        }

        static float ConvertChannel(float x, float black, float white, float expScale) {
            float range = white - black;
            if (range == 0) return 0;
            x = (x - black) / range;
            float exponent = expScale * range;
            float divider = UnityEngine.Mathf.Pow(2, exponent) - 1;
            if (divider == 0) return x;
            return (UnityEngine.Mathf.Pow(2, x * exponent) - 1) / divider;
        }
    }
}
