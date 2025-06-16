using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record Histogram3D {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			Shader.SetGlobalTexture("ImageMath_T0", Texture);
			ShaderSetGlobalUtils.SetGlobalVectorInt("ImageMath_V0", Size);
		}
	}
}