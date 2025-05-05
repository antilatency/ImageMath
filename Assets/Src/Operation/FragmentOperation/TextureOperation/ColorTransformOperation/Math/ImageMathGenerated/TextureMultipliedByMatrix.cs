using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record TextureMultipliedByMatrix {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			Shader.SetGlobalMatrix("ImageMath_M0", Multiplier);
		}
	}
}
