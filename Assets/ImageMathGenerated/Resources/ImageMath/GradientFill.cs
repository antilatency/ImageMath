using UnityEngine;
namespace ImageMath {
	public partial record GradientFill {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			Shader.SetGlobalVector("ImageMath_V2", ColorA);
			Shader.SetGlobalVector("ImageMath_V3", ColorB);
			Shader.SetGlobalVector("ImageMath_V4", PointA);
			Shader.SetGlobalVector("ImageMath_V5", PointB);
		}
	}
}
