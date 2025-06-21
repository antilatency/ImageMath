using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record LUT3DTransform {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			SetTexture("LUT", LUT);
			SetVector("DomainMin", DomainMin);
			SetVector("DomainMax", DomainMax);
		}
	}
}