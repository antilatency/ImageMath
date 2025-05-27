using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record TextureCompare {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			Shader.SetGlobalVector("ImageMath_V2", Reference);
			Shader.SetGlobalInt("ImageMath_I0", EqualOperation);
			Shader.SetGlobalFloat("ImageMath_F0", PreMultiplier);
			Shader.SetGlobalInt("ImageMath_I1", PostInverse);
		}
	}
}