using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record ColorFill {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			Shader.SetGlobalVector("ImageMath_V2", Color);
		}
	}
}