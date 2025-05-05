using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record EllipseFill {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			Shader.SetGlobalVector("ImageMath_V2", InnerColor);
			Shader.SetGlobalVector("ImageMath_V3", OuterColor);
			Shader.SetGlobalVector("ImageMath_V4", Center);
			Shader.SetGlobalVector("ImageMath_V5", Radius);
		}
	}
}
