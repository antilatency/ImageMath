using UnityEngine;
public partial record TestOperation {
	protected override void ApplyShaderParameters() {
		base.ApplyShaderParameters();
		Shader.SetGlobalInt("ImageMath_I0", IntValue1);
		Shader.SetGlobalInt("ImageMath_I1", IntValue2);
		Shader.SetGlobalVector("ImageMath_V3", VectorValue1);
		Shader.SetGlobalVector("ImageMath_V4", VectorValue2);
	}
}
