using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record LUT3DTransform {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			Shader.SetGlobalTexture("ImageMath_T1", LUT);
			Shader.SetGlobalVector("ImageMath_V2", DomainMin);
			Shader.SetGlobalVector("ImageMath_V3", DomainMax);
		}
	}
}