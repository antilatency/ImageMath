using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record TextureMultipliedByVector {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			Shader.SetGlobalVector("ImageMath_V2", Multiplier);
		}
	}
}
