using UnityEngine;
[ImageMath.Generated]
public partial record MulticompileOptionsTestOperation {
	protected override void ApplyShaderParameters() {
		base.ApplyShaderParameters();
		SetEnumKeyword("Color", Color);
		if (DrawCircle) EnableKeyword("DrawCircle"); else DisableKeyword("DrawCircle");
	}
}