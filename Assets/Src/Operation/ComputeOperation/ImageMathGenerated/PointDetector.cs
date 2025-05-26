using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record PointDetector {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			Shader.SetGlobalInt("ImageMath_I0", MaxSegmentsInRow);
			Shader.SetGlobalTexture("ImageMath_T0", Texture);
		}
	}
}