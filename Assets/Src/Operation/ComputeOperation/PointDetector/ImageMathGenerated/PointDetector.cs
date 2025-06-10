using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record PointDetector {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			Shader.SetGlobalVector("ImageMath_V0", Selector);
			Shader.SetGlobalFloat("ImageMath_F0", Threshold);
			Shader.SetGlobalInt("ImageMath_I0", MaxSegmentsInRow);
			Shader.SetGlobalTexture("ImageMath_T0", Texture);
		}
	}
}