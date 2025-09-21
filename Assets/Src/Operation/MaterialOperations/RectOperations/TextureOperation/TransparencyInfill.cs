using UnityEngine;
namespace ImageMath {
    [FilePath]
    public partial record TransparencyInfill : TextureOperation {
        public float Power { get; set; } = 1.0f;

        public TransparencyInfill(Texture texture) : base(texture) { }

        public TransparencyInfill() : base() { }

#if UNITY_EDITOR
        public static string GetFragmentShaderBody() => IncludeOrEmbed($"{nameof(TransparencyInfill)}.FragmentShaderBody.cginc");
#endif
    }
}
