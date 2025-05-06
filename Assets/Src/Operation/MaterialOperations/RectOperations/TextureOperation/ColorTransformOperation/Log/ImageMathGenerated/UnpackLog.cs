using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record UnpackLog {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			Shader.SetGlobalVector("ImageMath_V2", WhiteLevel);
			Shader.SetGlobalVector("ImageMath_V3", BlackLevel);
			Shader.SetGlobalVector("ImageMath_V4", ExponentScale);
		}
	}
}
