using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record UnpackPiecewiseLinearPow {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			SetFloat("InvThreshold", InvThreshold);
			SetFloat("InvLinearScale", InvLinearScale);
			SetFloat("InvLinearOffset", InvLinearOffset);
			SetFloat("InvPowInnerScale", InvPowInnerScale);
			SetFloat("InvPowInnerOffset", InvPowInnerOffset);
			SetFloat("InvPowExponent", InvPowExponent);
			SetFloat("InvPowOuterScale", InvPowOuterScale);
			SetFloat("InvPowOuterOffset", InvPowOuterOffset);
		}
	}
}