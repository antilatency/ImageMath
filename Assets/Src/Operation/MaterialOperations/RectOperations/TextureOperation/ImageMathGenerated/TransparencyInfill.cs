using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record TransparencyInfill {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			Shader.SetGlobalFloat("ImageMath_F0", Power);
		}
	}
}
