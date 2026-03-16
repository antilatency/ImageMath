using UnityEngine;
namespace ImageMath{
    [FilePath]
    public partial record FocusAssist : MaterialOperation {
        public Texture Texture { get; set; } = null;

        public float Multiplier { get; set; } = 1.0f;

        public FocusAssist(Texture texture) {
            Texture = texture;
        }

        public FocusAssist() : base() { }
#if UNITY_EDITOR
        public static string GetFragmentShaderBody() => IncludeOrEmbed($"{nameof(FocusAssist)}.FragmentShaderBody.cginc");
#endif
    }


}
