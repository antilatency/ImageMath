using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record LambertianSphereFill {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			Shader.SetGlobalVector("ImageMath_V2", Color);
			Shader.SetGlobalVector("ImageMath_V3", LightDirection);
		}
	}
}