using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record EllipseFill {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			SetVector("InnerColor", InnerColor);
			SetVector("OuterColor", OuterColor);
			SetVector("Center", Center);
			SetVector("Radius", Radius);
		}
	}
}