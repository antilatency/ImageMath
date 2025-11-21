using UnityEngine;
namespace ImageMath{
    [FilePath]
    public partial record DisplayLinearityValidation : MaterialOperation {


        public DisplayLinearityValidation() : base() { }
        
#if UNITY_EDITOR
        public static string GetFragmentShaderBody() => IncludeOrEmbed($"{nameof(DisplayLinearityValidation)}.FragmentShaderBody.cginc");
#endif
    }


}
