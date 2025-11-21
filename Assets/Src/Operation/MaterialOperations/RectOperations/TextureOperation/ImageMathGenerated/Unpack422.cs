using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record Unpack422 {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			SetFloat("Power", Power);
			SetEnumKeyword("Algorithm", Algorithm);
		}
	}
}