using UnityEngine;
#nullable enable

namespace ImageMath {
    [FilePath]
    public partial record Pack422 : TextureOperation {
        public Pack422(Texture? texture = null) : base(texture) {
        }

#if UNITY_EDITOR
        public static string GetFragmentShaderBody() => IncludeOrEmbed($"{nameof(Pack422)}.FragmentShaderBody.cginc");
#endif
    }
}
