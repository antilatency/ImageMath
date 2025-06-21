using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record LineFill {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			SetVector("Color", Color);
			SetFloat("LineWidth", LineWidth);
			SetFloat("LineSoftness", LineSoftness);
			SetVector("PointA", PointA);
			SetVector("PointB", PointB);
		}
	}
}