using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record PackYUV {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			SetMatrix("Matrix", Matrix);
			SetEnumKeyword("Standard", Standard);
		}
	}
}