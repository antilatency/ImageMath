using UnityEngine;
namespace ImageMath {
	public partial record FragmentOperation {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			Shader.SetGlobalVector("ImageMath_V0", Position);
			Shader.SetGlobalVector("ImageMath_V1", Size);
		}
	}
}
