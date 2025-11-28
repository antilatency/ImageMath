using UnityEngine;
using ImageMath;
using ImageMath.Views;
using ImageMath.ScriptableObjects;


#if UNITY_EDITOR
using UnityEditor;
#endif
#nullable enable

[ExecuteAlways]
public class SpectrumsTest : MonoBehaviour{
	public ScriptableSpectrum ReferenceSpectrum;
	public ScriptableSpectrum TestSpectrum;

	public float BlackbodyTemperature = 6500f;

	public float SimilarityIndex;

	void Update() {
		var reference = ReferenceSpectrum.Value.Normalized();
		//Spectrums.CreateBlackbodySpectrum(BlackbodyTemperature,200,800).Normalized()* RefMultiplier;
		var blackBodyView = ImageMath.Views.SpectrumView.GetByName("BlackbodySpectrum");
		blackBodyView.Spectrum = reference;

		var test = TestSpectrum.Value.Normalized();
		var testView = ImageMath.Views.SpectrumView.GetByName("TestSpectrum");
		testView.Spectrum = test;

		SimilarityIndex = test.SpectrumSimularityIndex(reference);

	}
	
#if UNITY_EDITOR
	public void OnDrawGizmos() {
		Handles.matrix = transform.localToWorldMatrix;

		
		Handles.matrix = Matrix4x4.identity;		
	}
#endif
}