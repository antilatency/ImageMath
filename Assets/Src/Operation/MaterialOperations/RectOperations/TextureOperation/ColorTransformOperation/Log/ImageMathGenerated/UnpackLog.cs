using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record UnpackLog {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			Shader.SetGlobalFloat("ImageMath_F0", BlackLevel);
			Shader.SetGlobalFloat("ImageMath_F1", ExponentScale);
			Shader.SetGlobalFloat("ImageMath_F2", Multiplier);
		}
	}
}