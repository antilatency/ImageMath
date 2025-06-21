using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record PointDetector {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			SetVector("Selector", Selector);
			SetFloat("Threshold", Threshold);
			SetInt("MaxSegmentsInRow", MaxSegmentsInRow);
			SetTexture("Texture", Texture);
		}
	}
}