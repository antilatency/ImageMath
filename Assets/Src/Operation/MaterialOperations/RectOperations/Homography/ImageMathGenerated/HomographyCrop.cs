using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record HomographyCrop {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			SetMatrix("HomographyMatrix", HomographyMatrix);
		}
	}
}