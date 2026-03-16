using UnityEngine;
using ImageMath;
using ImageMath.Views;
using System;


#if UNITY_EDITOR
using UnityEditor;
#endif
#nullable enable

[ExecuteAlways]
public class DecklinkTest : MonoBehaviour{


	public Unpack422.UpsamplingAlgorithm Algorithm = Unpack422.UpsamplingAlgorithm.LinearInterpolation;

	public bool FlipVertically = false;
	[Range(0,8)]
	public int ErrorPowerOf10 = 2;

	public float FocusAssistMultiplier = 10.0f;

	void Update() {
		var inputTexture = TextureView.GetByName("InputTexture").Texture;
		if (inputTexture == null) {
			return;
		}

		var unpackedTextureDimensions = Unpack422.GetOutputTextureDimensions(new Vector2Int(inputTexture.width, inputTexture.height), Layout422.Cb0Y0Cr0Y1);

		var unpacked = TextureView.GetByName("UnpackedTexture").ResizeRenderTexture(unpackedTextureDimensions.x, unpackedTextureDimensions.y);

		new Unpack422(inputTexture) {
			Algorithm = Algorithm,
			FlipVertically = FlipVertically
		}.AssignTo(unpacked);

		var rgbTexture = TextureView.GetByName("RGBTexture").ResizeRenderTexture(unpackedTextureDimensions.x, unpackedTextureDimensions.y);
		YUV.Unpack(unpacked).AssignTo(rgbTexture);


		var referenceTexture = TextureView.GetByName("ReferenceTexture").Texture;
		if (referenceTexture == null) {
			return;
		}

		var errorTexture = TextureView.GetByName("ErrorTexture").ResizeRenderTexture(unpackedTextureDimensions.x, unpackedTextureDimensions.y);


		new SquaredDiff(referenceTexture, rgbTexture, Mathf.Pow(10, ErrorPowerOf10)).AssignTo(errorTexture);
		errorTexture.ClearAlpha();


		var clippingDetection = TextureView.GetByName("ClippingDetection").ResizeRenderTexture(unpackedTextureDimensions.x, unpackedTextureDimensions.y);
		new ClippingDetection(rgbTexture).AssignTo(clippingDetection);


		var linear = TextureView.GetByName("LinearTexture").ResizeRenderTexture(unpackedTextureDimensions.x, unpackedTextureDimensions.y);
		TransferFunctions.UnpackSRGB(rgbTexture).AssignTo(linear);

		var focusAssist = TextureView.GetByName("FocusAssist").ResizeRenderTexture(unpackedTextureDimensions.x, unpackedTextureDimensions.y);
		new FocusAssist(linear) {
			Multiplier = FocusAssistMultiplier
		}.AssignTo(focusAssist);

		new TextureMultipliedByVector(linear, 0.25f).AddTo(focusAssist);

    }
	
#if UNITY_EDITOR
	public void OnDrawGizmos() {
		Handles.matrix = transform.localToWorldMatrix;
		
		Handles.matrix = Matrix4x4.identity;		
	}
#endif
}