using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record TextureMultiplyAdd {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			Shader.SetGlobalMatrix("ImageMath_M0", Multiplier);
			Shader.SetGlobalVector("ImageMath_V2", Increment);
		}
	}
}