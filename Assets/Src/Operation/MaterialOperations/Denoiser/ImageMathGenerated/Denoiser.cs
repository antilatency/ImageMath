using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record Denoiser {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			SetTexture("Texture", Texture);
			SetFloat("Power", Power);
			SetInt("Size", Size);
		}
	}
}