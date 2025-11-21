using UnityEngine;
#nullable enable

namespace ImageMath {
    [FilePath]
    public partial record Unpack422 : TextureOperation {

        public enum UpsamplingAlgorithm {
            NearestNeighbor,
            LinearInterpolation,
            GradientBased
        }
        public float Power { get; set; } = 1.0f;

        [MulticompileOptions]
        public UpsamplingAlgorithm Algorithm { get; set; } = UpsamplingAlgorithm.LinearInterpolation;

        public Unpack422(Texture? texture = null) : base(texture) {
        }

#if UNITY_EDITOR
        public static string GetFragmentShaderBody() => IncludeOrEmbed($"{nameof(Unpack422)}.FragmentShaderBody.cginc");
#endif
    }

}
