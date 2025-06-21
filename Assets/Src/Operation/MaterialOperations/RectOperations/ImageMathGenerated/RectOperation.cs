using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record RectOperation {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			SetVector("Position", Position);
			SetVector("Size", Size);
		}
	}
}