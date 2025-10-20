using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record PolynomialColorTransformOperation {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			SetFloatArray("Coefficients", ExpandArray(Coefficients,16));
			Shader.SetGlobalInt("Coefficients_Size", Coefficients.Length);
		}
	}
}