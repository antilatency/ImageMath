using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record NormalizableColorTransformOperation {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			SetFloat("WhiteLevel", WhiteLevel);
			SetEnumKeyword("WUsage", WUsage);
		}
	}
}