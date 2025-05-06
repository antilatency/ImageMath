using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record RectOperation {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			Shader.SetGlobalVector("ImageMath_V0", Position);
			Shader.SetGlobalVector("ImageMath_V1", Size);
		}
	}
}
