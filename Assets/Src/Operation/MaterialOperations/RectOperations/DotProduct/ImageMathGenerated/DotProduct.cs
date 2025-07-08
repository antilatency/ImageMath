using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record DotProduct {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			SetVectorArray("Weights", ExpandArray(Weights,16));
			SetEnumKeyword("Count", Count, 1, 16);
		}
	}
}