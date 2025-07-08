using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record Denoiser {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			SetTexture("Texture", Texture);
			SetFloat("Level", Level);
			SetEnumKeyword("Size", Size);
			if (RenderDelta) EnableKeyword("RenderDelta"); else DisableKeyword("RenderDelta");
		}
	}
}