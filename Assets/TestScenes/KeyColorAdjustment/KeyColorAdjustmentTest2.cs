using UnityEngine;
using ImageMath;
using System.Linq;
using UnityEngine.Experimental.Rendering;
using System.Collections.Generic;





#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class KeyColorAdjustmentTest2 : MonoBehaviour {

	[Range(-1, 1)]
	public float RelativeChromaAdjustment = 0.0f;
	[Range(-1, 1)]
	public float RelativeBrightnessAdjustment = 0.0f;

	public bool AverageColorIsAdjustmentZero = true;

	[Range(-1, 1)]
	public float AbsoluteChromaAdjustment = 0;
	[Range(-1, 1)]	
	public float AbsoluteBrightnessAdjustment = 0;

	public Color[] KeyColors = new Color[] {
		new Color(0.5f, 0.8f, 0.5f, 1.0f),
		new Color(0.5f, 0.9f, 0.5f, 1.0f),
		new Color(0.5f, 0.7f, 0.5f, 1.0f)
	};

	public Color AdjustedColor;

	public Material DisplayMaterial;


	public Texture2D ChromasAndColorsTexture;

	//first color is adjusted key color
	public void UpdateColorsTexture(Vector4[] chromasAndColors) {
		var textureWidth = chromasAndColors.Length / 2;
		if (!ChromasAndColorsTexture || ChromasAndColorsTexture.width != textureWidth) {
			if (ChromasAndColorsTexture) { 
				DestroyImmediate(ChromasAndColorsTexture);
			}
			ChromasAndColorsTexture = new Texture2D(textureWidth, 2, GraphicsFormat.R32G32B32A32_SFloat, TextureCreationFlags.None);
			ChromasAndColorsTexture.filterMode = FilterMode.Point;
			ChromasAndColorsTexture.wrapMode = TextureWrapMode.Clamp;
		}
		if (DisplayMaterial == null) {
			return;
		}
		ChromasAndColorsTexture.SetPixelData(chromasAndColors,0);
		ChromasAndColorsTexture.Apply();
		DisplayMaterial.SetTexture("_ChromasAndColorsTexture", ChromasAndColorsTexture);
	}

	float Curve(float x, float resultForZero) {
		//x from -1 to 1
		//result from 0 to 1
		float power = Mathf.Log(resultForZero) / Mathf.Log(0.5f);
		return Mathf.Pow(0.5f * x + 0.5f, power);
	}

	public static float InverseLerpUnclamped(float a, float b, float value) {
		if (a == b) return 0f;
		return (value - a) / (b - a);
	}

	//ondrawgizmos
#if UNITY_EDITOR
	void OnDrawGizmos() {
		Handles.matrix = transform.localToWorldMatrix;

		var averageColor = new Color(0f, 0f, 0f, 0f);
		foreach (var keyColor in KeyColors) {
			averageColor += keyColor;
		}
		averageColor /= KeyColors.Length;



		var xScale = Mathf.Sqrt(2f) / Mathf.Sqrt(3f);

		Vector3 AxisY = new Vector3(1f, 1f, 1f);
		Vector3 AxisZ = Vector3.Cross(AxisY, averageColor.ToVector3()).normalized * xScale;
		Vector3 AxisX = Vector3.Cross(AxisZ, AxisY).normalized * xScale;

		DisplayMaterial.SetVector("_GlobalAxisX", AxisX);
		DisplayMaterial.SetVector("_GlobalAxisY", AxisY);


		Matrix4x4 chromaToColor = new Matrix4x4(
			new Vector4(AxisX.x, AxisX.y, AxisX.z, 0f),
			new Vector4(AxisY.x, AxisY.y, AxisY.z, 0f),
			new Vector4(AxisZ.x, AxisZ.y, AxisZ.z, 0f),
			new Vector4(0f, 0f, 0f, 1f)
		);
		Matrix4x4 colorToChroma = chromaToColor.inverse;

		var averageChroma = colorToChroma * averageColor.ToVector3();
		averageChroma.z = 0f; // Ensure z is zero for 2D representation

		var chromas = KeyColors.Select(keyColor => (Vector2)(colorToChroma * keyColor.ToVector3())).ToArray();
		var minChroma = chromas.Aggregate((a, b) => Vector2.Min(a, b));
		var maxChroma = chromas.Aggregate((a, b) => Vector2.Max(a, b));

		//draw chroma rectangle
		var rect = new Rect(
			minChroma.x, minChroma.y,
			maxChroma.x - minChroma.x, maxChroma.y - minChroma.y
		);

		Handles.color = Color.white;
		Handles.DrawSolidRectangleWithOutline(
			rect,
			new Color(1f, 1f, 1f, 0.0f),
			Color.white
		);
		DisplayMaterial.SetVector("_ChromaRect", new Vector4(rect.xMin, rect.yMin, rect.width, rect.height));
			
		//unit rectangle
		Handles.DrawSolidRectangleWithOutline(
			new Rect(0f, 0f, 1f, 1f),
			new Color(1f, 1f, 1f, 0.0f),
			Color.white
		);

		var rca = RelativeChromaAdjustment;
		var rba = RelativeBrightnessAdjustment;
		if (AverageColorIsAdjustmentZero) {
			var averageChromaRelativeX = Mathf.InverseLerp(minChroma.x, maxChroma.x, averageChroma.x);
			var averageChromaRelativeY = Mathf.InverseLerp(minChroma.y, maxChroma.y, averageChroma.y);
			rca = Curve(rca, averageChromaRelativeX);
			rba = Curve(rba, averageChromaRelativeY);
		}
		else {
			rca = rca * 0.5f + 0.5f;
			rba = rba * 0.5f + 0.5f;
		}



		var adjustedChroma = new Vector3(
			Mathf.Lerp(minChroma.x, maxChroma.x, rca) + AbsoluteChromaAdjustment,
			Mathf.Lerp(minChroma.y, maxChroma.y, rba) + AbsoluteBrightnessAdjustment,
			 0f);

		AdjustedColor = chromaToColor * adjustedChroma;
		AdjustedColor.a = 1.0f; // Ensure alpha is set to 1




		for (int i = 0; i < KeyColors.Length; i++) {
			var keyColor = KeyColors[i];
			var chroma = chromas[i];
			Handles.color = keyColor;
			Handles.DrawWireDisc(chroma, Vector3.forward, 0.01f);
		}
		Handles.color = averageColor;
		Handles.DrawWireDisc(colorToChroma * averageColor.ToVector3(), Vector3.forward, 0.02f);

		Handles.color = AdjustedColor;
		Handles.DrawWireDisc(adjustedChroma, Vector3.forward, 0.03f);

		Handles.matrix = Matrix4x4.identity;


		var colorsOffset = 2 + KeyColors.Length;
		var chromasAndColors = new Vector4[2 * colorsOffset];
		chromasAndColors[0] = adjustedChroma;
		chromasAndColors[colorsOffset] = AdjustedColor.ToVector4();

		chromasAndColors[1] = averageChroma;
		chromasAndColors[colorsOffset + 1] = averageColor.ToVector4();
		for (int i = 0; i < KeyColors.Length; i++) {
			var keyColor = KeyColors[i];
			var chroma = chromas[i];
			//Vector2 relativeChroma = ToRelativeChroma(chroma);

			chromasAndColors[2 + i] = new Vector4(chroma.x, chroma.y, 0f, 0f);
			chromasAndColors[colorsOffset + 2 + i] = keyColor.ToVector4();
		}

		UpdateColorsTexture(chromasAndColors);

	}
#endif
}
