using UnityEngine;
namespace ImageMath{
    [FilePath]
    public partial record FlatLUT3DIdentity : MaterialOperation {
        public FlatLUT3DIdentity() : base() { }
#if UNITY_EDITOR
        public static string GetFragmentShaderBody() => IncludeOrEmbed($"{nameof(FlatLUT3DIdentity)}.FragmentShaderBody.cginc");
#endif
    }


}
