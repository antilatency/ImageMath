using UnityEngine;
using ImageMath;
using ImageMath.Views;
using static ImageMath.Static;


#if UNITY_EDITOR
using UnityEditor;
#endif
#nullable enable

[ExecuteAlways]
public class YUV422Test : MonoBehaviour{


	public float NoiseAmplitude = 0.01f;
	public InspectorButton _GenerateTestImage;
	public void GenerateTestImage() {
		//load Shader "Custom/VertexColor"
		var shader = Shader.Find("Custom/VertexColor");
		var material = new Material(shader);
		material.SetPass(0);

		int targetSize = 256;
		int renderSize = 4096;

		using var renderTexture = GetTempRenderTexture(renderSize, renderSize);

		//draw a rectangle
		var previousRenderTexture = RenderTexture.active;

		RenderTexture.active = renderTexture;
		GL.Clear(false, true, Color.blue);
		GL.PushMatrix();
		GL.LoadPixelMatrix(0, renderSize, renderSize, 0);

		int numTriangles = 1024;
		GL.Begin(GL.TRIANGLES);
		for (int i = 0; i < numTriangles; i++) {
			float ni = (float)i / (numTriangles - 1);
			Vector2 center = new Vector2(Random.value * (renderSize - 1), Random.value * (renderSize - 1));
			float radius = Mathf.Lerp(renderSize, 0.01f * renderSize, ni);
			Color color = Random.ColorHSV();
			GL.Color(color);
			for (int j = 0; j < 3; j++) {
				Vector2 offset = Random.insideUnitCircle * radius;
				Vector2 vertex = center + offset;
				GL.Vertex3(vertex.x, vertex.y, 0);
			}

		}
		GL.End();
		GL.PopMatrix();
		RenderTexture.active = previousRenderTexture;

		/*var currentSize = renderSize;
		var currentRenderTexture = renderTexture.Value;*/

		using var renderTexture2048 = GetTempRenderTexture(2048, 2048);
		new TextureCopy(renderTexture.Value).AssignTo(renderTexture2048);

		using var renderTexture1024 = GetTempRenderTexture(1024, 1024);
		new TextureCopy(renderTexture2048).AssignTo(renderTexture1024);

		using var renderTexture512 = GetTempRenderTexture(512, 512);
		new TextureCopy(renderTexture1024).AssignTo(renderTexture512);

		var inputTexture = TextureView.GetByName("InputTexture").ResizeRenderTexture(targetSize, targetSize);
		inputTexture.filterMode = FilterMode.Point;
		new TextureCopy(renderTexture512).AssignTo(inputTexture);

		new GaussianNoiseRGBAFill(NoiseAmplitude).AddTo(inputTexture);

		new ColorFill(new Vector4(0,0,0,0)).MaxTo(inputTexture);

	}




	public Unpack422.UpsamplingAlgorithm Algorithm = Unpack422.UpsamplingAlgorithm.LinearInterpolation;

	[Range(0.0f, 0.2f)]
	public float AntiNoiseProtection = 0.01f;

	[Range(0.0f, 500.0f)]
	public float ErrorMultiplier = 100.0f;
	public float Error;
	public float Error2;
	void Update() {

		var inputTexture = TextureView.GetByName("InputTexture").Texture;
		if (inputTexture == null) return;

		var transferFunctionAppliedTexture = TextureView.GetByName("TransferFunctionAppliedTexture").ResizeRenderTexture(inputTexture.width, inputTexture.height);
		new PackSRGB(inputTexture).AssignTo(transferFunctionAppliedTexture);

		var yuvTexture = TextureView.GetByName("YUVTexture").ResizeRenderTexture(inputTexture.width, inputTexture.height);
		YUV.Pack(transferFunctionAppliedTexture).AssignTo(yuvTexture);

		var packedTextureDimensions = Pack422.GetOutputTextureDimensions(new Vector2Int(inputTexture.width, inputTexture.height), Layout422.Cb0Y0Cr0Y1);

		var packed422Texture = TextureView.GetByName("Packed422Texture").ResizeRenderTexture(packedTextureDimensions.x, packedTextureDimensions.y);
		packed422Texture.filterMode = FilterMode.Point;
		new Pack422(yuvTexture).AssignTo(packed422Texture);


		var unpacked422Texture = TextureView.GetByName("Unpacked422Texture").ResizeRenderTexture(inputTexture.width, inputTexture.height);
		unpacked422Texture.filterMode = FilterMode.Point;
		new Unpack422(packed422Texture) {
			Algorithm = Algorithm,
			AntiNoiseProtection = AntiNoiseProtection
		}.AssignTo(unpacked422Texture);
		unpacked422Texture.filterMode = FilterMode.Point;

		var rgbTexture = TextureView.GetByName("RGBTexture").ResizeRenderTexture(inputTexture.width, inputTexture.height);
		rgbTexture.filterMode = FilterMode.Point;
		YUV.Unpack(unpacked422Texture).AssignTo(rgbTexture);

		var transferFunctionRemovedTexture = TextureView.GetByName("TransferFunctionRemovedTexture").ResizeRenderTexture(inputTexture.width, inputTexture.height);
		new UnpackSRGB(rgbTexture).AssignTo(transferFunctionRemovedTexture);


		var negativeCheckTexture = TextureView.GetByName("NegativeCheckTexture").ResizeRenderTexture(inputTexture.width, inputTexture.height);
		new ImageMath.TextureCompare(yuvTexture, TextureCompare.CompareOperation.Less, 0).AssignTo(negativeCheckTexture);

		var errorTexture = TextureView.GetByName("ErrorTexture").ResizeRenderTexture(inputTexture.width, inputTexture.height);
		new SquaredDiff(inputTexture, transferFunctionRemovedTexture, ErrorMultiplier).AssignTo(errorTexture);
		errorTexture.ClearAlpha();

		var error = errorTexture.AverageWeightedByAlpha_Divided();
		Error = Mathf.Sqrt(error.x + error.y + error.z);

		var yuv2 = TextureView.GetByName("YUV2").ResizeRenderTexture(inputTexture.width, inputTexture.height);
		YUV.Pack(transferFunctionRemovedTexture).AssignTo(yuv2);

		var new422 = TextureView.GetByName("New422").ResizeRenderTexture(packedTextureDimensions.x, packedTextureDimensions.y);

		new Pack422(yuv2).AssignTo(new422);

		var unpacked2 = TextureView.GetByName("Unpacked2").ResizeRenderTexture(inputTexture.width, inputTexture.height);
		new Unpack422(new422) {
			Algorithm = Algorithm,
			AntiNoiseProtection = AntiNoiseProtection
		}.AssignTo(unpacked2);

		var rgb2 = TextureView.GetByName("RGB2").ResizeRenderTexture(inputTexture.width, inputTexture.height);
		YUV.Unpack(unpacked2).AssignTo(rgb2);

		var error2Texture = TextureView.GetByName("Error2Texture").ResizeRenderTexture(inputTexture.width, inputTexture.height);
		new SquaredDiff(inputTexture, rgb2, ErrorMultiplier).AssignTo(error2Texture);
		error2Texture.ClearAlpha();


		var error2 = error2Texture.AverageWeightedByAlpha_Divided();
		Error2 = Mathf.Sqrt(error2.x + error2.y + error2.z);


    }
	
#if UNITY_EDITOR
	public void OnDrawGizmos() {
		Handles.matrix = transform.localToWorldMatrix;
		
		Handles.matrix = Matrix4x4.identity;		
	}
#endif
}