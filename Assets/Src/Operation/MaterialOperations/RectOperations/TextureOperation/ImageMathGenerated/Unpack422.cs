using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record Unpack422 {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			SetFloat("AntiNoiseProtection", AntiNoiseProtection);
			SetEnumKeyword("Layout", Layout);
			if (FlipVertically) EnableKeyword("FlipVertically"); else DisableKeyword("FlipVertically");
			SetEnumKeyword("Algorithm", Algorithm);
		}
	}
}