using UnityEngine;
using ImageMath;
using ImageMath.Views;

#if UNITY_EDITOR
using UnityEditor;
#endif
#nullable enable

[ExecuteAlways]
public class SpectrumsTest : MonoBehaviour{

	public float BlackbodyTemperature = 6500f;

	void Update() {
		var blackBodyView = ImageMath.Views.SpectrumView.GetByName("BlackbodySpectrum");
		blackBodyView.Spectrum = Spectrums.CreateBlackbodySpectrum(BlackbodyTemperature).Normalized();
    }
	
#if UNITY_EDITOR
	public void OnDrawGizmos() {
		Handles.matrix = transform.localToWorldMatrix;

		
		Handles.matrix = Matrix4x4.identity;		
	}
#endif
}