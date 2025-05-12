using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record ReductionOperation {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			Shader.SetGlobalTexture("ImageMath_T0", Texture);
		}
	}
}