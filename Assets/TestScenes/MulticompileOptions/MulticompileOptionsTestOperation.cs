using ImageMath;
using UnityEngine;

[FilePath]
public partial record MulticompileOptionsTestOperation : RectOperation {
    [MulticompileOptions]
    public ColorEnum Color { get; set; } = ColorEnum.Red;

    [MulticompileOptions]
    public bool DrawCircle { get; set; } = true;


    public MulticompileOptionsTestOperation() {
    }
#if UNITY_EDITOR
    public static string GetFragmentShaderBody() => IncludeOrEmbed("MulticompileOptionsTestOperation.FragmentShaderBody.cginc");
#endif
}
