using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record PiecewiseLinearPow {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			SetFloat("Threshold", Threshold);
			SetFloat("LinearScale", LinearScale);
			SetFloat("LinearOffset", LinearOffset);
			SetFloat("PowInnerScale", PowInnerScale);
			SetFloat("PowInnerOffset", PowInnerOffset);
			SetFloat("PowExponent", PowExponent);
			SetFloat("PowOuterScale", PowOuterScale);
			SetFloat("PowOuterOffset", PowOuterOffset);
		}
	}
}