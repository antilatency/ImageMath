using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record GradientFill {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			SetVector("ColorA", ColorA);
			SetVector("ColorB", ColorB);
			SetVector("PointA", PointA);
			SetVector("PointB", PointB);
		}
	}
}