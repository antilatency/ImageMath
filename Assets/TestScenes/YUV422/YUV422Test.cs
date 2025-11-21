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

	}


	public Unpack422.UpsamplingAlgorithm Algorithm = Unpack422.UpsamplingAlgorithm.LinearInterpolation;

	public float Power = 1.0f;
	public float Error;
	void Update() {

		var inputTexture = TextureView.GetByName("InputTexture").Texture;
		if (inputTexture == null) return;

		var transferFunctionAppliedTexture = TextureView.GetByName("TransferFunctionAppliedTexture").ResizeRenderTexture(inputTexture.width, inputTexture.height);
		new PackSRGB(inputTexture).AssignTo(transferFunctionAppliedTexture);

		var yuvTexture = TextureView.GetByName("YUVTexture").ResizeRenderTexture(inputTexture.width, inputTexture.height);
		YUV.Pack(transferFunctionAppliedTexture).AssignTo(yuvTexture);
		/*new TextureMultipliedByMatrix(inputTexture,
			new Matrix4x4(
				new Vector4(1/3.0f, 0, 0, 0),
				new Vector4(1/3.0f, 1, 0, 0),
				new Vector4(1/3.0f, 0, 1, 0),
				new Vector4(0, 0, 0, 1)
			)
		).AssignTo(yuvTexture);*/

		var packed422Texture = TextureView.GetByName("Packed422Texture").ResizeRenderTexture(inputTexture.width, 2 * inputTexture.height);
		packed422Texture.filterMode = FilterMode.Point;
		new Pack422(yuvTexture).AssignTo(packed422Texture);


		var unpacked422Texture = TextureView.GetByName("Unpacked422Texture").ResizeRenderTexture(inputTexture.width, inputTexture.height);
		unpacked422Texture.filterMode = FilterMode.Point;
		new Unpack422(packed422Texture) {
			Algorithm = Algorithm,
			Power = Power
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
		new SquaredDiff(inputTexture, transferFunctionRemovedTexture, 100).AssignTo(errorTexture);
		errorTexture.ClearAlpha();

		var error = errorTexture.AverageWeightedByAlpha_Divided();
		Error = Mathf.Sqrt(error.x + error.y + error.z);

		var finetunedTexture = TextureView.GetByName("FinetunedTexture").ResizeRenderTexture(inputTexture.width, inputTexture.height);
		new Finetune422(transferFunctionRemovedTexture, rgbTexture).AssignTo(finetunedTexture);

		var errorTexture2 = TextureView.GetByName("ErrorTexture2").ResizeRenderTexture(inputTexture.width, inputTexture.height);
		new SquaredDiff(inputTexture, finetunedTexture, 100).AssignTo(errorTexture2);


    }
	
#if UNITY_EDITOR
	public void OnDrawGizmos() {
		Handles.matrix = transform.localToWorldMatrix;
		
		Handles.matrix = Matrix4x4.identity;		
	}
#endif
}