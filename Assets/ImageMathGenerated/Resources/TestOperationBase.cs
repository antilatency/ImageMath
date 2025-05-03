using UnityEngine;
public partial record TestOperationBase {
	protected override void ApplyShaderParameters() {
		base.ApplyShaderParameters();
		Shader.SetGlobalVector("ImageMath_V2", VectorValue0);
	}
}
