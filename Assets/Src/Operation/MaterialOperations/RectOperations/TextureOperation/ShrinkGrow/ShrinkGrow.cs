using UnityEngine;
namespace ImageMath{
    [FilePath]
    public partial record ShrinkGrow : TextureOperation {
        public float Power { get; set; } = 1.0f;

        public ShrinkGrow(Texture texture) : base(texture) { }

        public ShrinkGrow() : base() { }

#if UNITY_EDITOR
        public static string GetFragmentShaderBody() => IncludeOrEmbed($"{nameof(ShrinkGrow)}.FragmentShaderBody.cginc");
#endif

    }


}
