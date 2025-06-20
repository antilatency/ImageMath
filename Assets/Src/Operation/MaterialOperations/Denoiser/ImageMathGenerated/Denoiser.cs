using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record Denoiser {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			Shader.SetGlobalTexture("ImageMath_T0", Texture);
			Shader.SetGlobalFloat("ImageMath_F0", Power);
			Shader.SetGlobalInt("ImageMath_I0", Size);
		}
	}
}