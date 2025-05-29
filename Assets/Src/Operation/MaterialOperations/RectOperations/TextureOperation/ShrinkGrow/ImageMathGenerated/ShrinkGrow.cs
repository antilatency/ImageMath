using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record ShrinkGrow {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			Shader.SetGlobalFloat("ImageMath_F0", Power);
		}
	}
}