using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record PackLog {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			Shader.SetGlobalFloat("ImageMath_F0", BlackLevel);
			Shader.SetGlobalFloat("ImageMath_F1", InverseExponentScale);
			Shader.SetGlobalFloat("ImageMath_F2", Multiplier);
		}
	}
}