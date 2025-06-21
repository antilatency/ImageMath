using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record ShrinkGrow {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			SetFloat("Power", Power);
		}
	}
}