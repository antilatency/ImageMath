using UnityEngine;
namespace ImageMath {
	public partial record LineFill {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			Shader.SetGlobalVector("ImageMath_V2", Color);
			Shader.SetGlobalFloat("ImageMath_F0", LineWidth);
			Shader.SetGlobalFloat("ImageMath_F1", LineSoftness);
			Shader.SetGlobalVector("ImageMath_V3", PointA);
			Shader.SetGlobalVector("ImageMath_V4", PointB);
		}
	}
}
