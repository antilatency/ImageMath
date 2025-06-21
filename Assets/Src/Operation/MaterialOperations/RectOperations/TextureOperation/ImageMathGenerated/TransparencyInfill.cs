using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record TransparencyInfill {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			SetFloat("Power", Power);
		}
	}
}