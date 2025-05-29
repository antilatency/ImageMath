using UnityEngine;
using ImageMath;
using ImageMath.Views;

#if UNITY_EDITOR
using UnityEditor;
#endif
#nullable enable


public static class Chroma {
	static readonly Vector3 ChromaToColorZ = new Vector3(1f, 1f, 1f) / 3f;
	static readonly Vector3 ChromaToColorZNormalized = Vector3.Normalize(new Vector3(1f, 1f, 1f));
	static readonly Vector3 Green = new Vector3(0f, 1f, 0f);

	static readonly Vector3 ChromaToColorY = Green - ChromaToColorZNormalized * Vector3.Dot(Green, ChromaToColorZNormalized);
	static readonly Vector3 ChromaToColorX = Vector3.Normalize(Vector3.Cross(ChromaToColorY, ChromaToColorZ)) * ChromaToColorY.magnitude;
	public static readonly float ChromaRadius = new Vector2(ChromaToColorX.x,ChromaToColorX.y).magnitude;

	public static readonly Matrix4x4 ChromaToColor = new Matrix4x4(
		new Vector4(ChromaToColorX.x, ChromaToColorX.y, ChromaToColorX.z, 0f),
		new Vector4(ChromaToColorY.x, ChromaToColorY.y, ChromaToColorY.z, 0f),
		new Vector4(ChromaToColorZ.x, ChromaToColorZ.y, ChromaToColorZ.z, 0f),
		new Vector4(0f, 0f, 0f, 1f)
	);

	public static readonly Matrix4x4 ColorToChroma = ChromaToColor.inverse;



	public static Vector4 ConvertColorToChroma(this Color color) {
		return ChromaToColor.transpose * color;
	}
	public static Color ConvertChromaToColor(this Vector4 chroma) {
		return ColorToChroma.transpose * chroma;
	}

    
}


[ExecuteAlways]
public class KeyColorAdjustmentTest : MonoBehaviour{
	
	public Color KeyColor = new Color(0.5f, 0.8f, 0.5f, 1.0f);
	public Color AdjustedColor;
	
	[Range(-1, 1)]
	public float ChromaAdjustment = 0.0f;
	[Range(-1,1)]
	public float BrightnessAdjustment = 0.0f;




	void Update() {

    }

	float Curve(float x, float resultForZero) {
		//x from -1 to 1
		//result from 0 to 1
		float power = Mathf.Log(resultForZero) / Mathf.Log(0.5f);
		return Mathf.Pow(0.5f * x + 0.5f, power);
	}

	float CurveLinearSegments(float x, float resultForZero) {
		// Clamp x just in case
		x = Mathf.Clamp(x, -1f, 1f);

		if (x < 0f) {
			// Line from (-1, 0) to (0, resultForZero)
			return Mathf.Lerp(0f, resultForZero, (x + 1f) / 1f);
		} else {
			// Line from (0, resultForZero) to (1, 1)
			return Mathf.Lerp(resultForZero, 1f, x / 1f);
		}
	}

	public Color SaturatedColor;
	public Vector4 _Chroma;
	
	public static Vector3 ColorToHSL(Color color) {
		float r = color.r;
		float g = color.g;
		float b = color.b;

		float max = Mathf.Max(r, g, b);
		float min = Mathf.Min(r, g, b);
		float h = 0f, s, l = (max + min) / 2f;

		float d = max - min;
		if (Mathf.Approximately(d, 0f))
		{
			h = s = 0f; // achromatic
		}
		else
		{
			s = l < 0.5f ? d / (max + min) : d / (2f - max - min);

			if (Mathf.Approximately(max, r))
				h = (g - b) / d + (g < b ? 6f : 0f);
			else if (Mathf.Approximately(max, g))
				h = (b - r) / d + 2f;
			else
				h = (r - g) / d + 4f;

			h /= 6f;
		}

		return new Vector3(h, s, l); // H, S, L ∈ [0, 1]
	}


	public static Color HSLToColor(Vector3 hsl) {
		float h = hsl.x;
		float s = hsl.y;
		float l = hsl.z;

		float r, g, b;

		if (Mathf.Approximately(s, 0f))
		{
			r = g = b = l; // achromatic
		}
		else
		{
			float q = l < 0.5f ? l * (1f + s) : l + s - l * s;
			float p = 2f * l - q;

			r = HueToRGB(p, q, h + 1f / 3f);
			g = HueToRGB(p, q, h);
			b = HueToRGB(p, q, h - 1f / 3f);
		}

		return new Color(r, g, b, 1f);
	}

	private static float HueToRGB(float p, float q, float t) {
		if (t < 0f) t += 1f;
		if (t > 1f) t -= 1f;
		if (t < 1f / 6f) return p + (q - p) * 6f * t;
		if (t < 1f / 2f) return q;
		if (t < 2f / 3f) return p + (q - p) * (2f / 3f - t) * 6f;
		return p;
	}

	public static Color CalcSaturatedColor(Color color)
    {
        float r = color.r;
        float g = color.g;
        float b = color.b;

        float sum = r + g + b;
        float min = Mathf.Min(r, Mathf.Min(g, b));

        float r1 = r - min;
        float g1 = g - min;
        float b1 = b - min;

        float sumNonZero = r1 + g1 + b1;
        if (sumNonZero == 0f)
            return new Color(0f, 0f, 0f, color.a);

        float scale = sum / sumNonZero;

		var result = new Color(r1 * scale, g1 * scale, b1 * scale, color.a);
		var maxChannel = Mathf.Max(result.r, Mathf.Max(result.g, result.b));
		if (maxChannel >1f)
			result /= maxChannel;

		return result;
    }



#if UNITY_EDITOR
	public void OnDrawGizmos() {
		Handles.matrix = transform.localToWorldMatrix;

		var linearColor = KeyColor;

		var chroma = linearColor.ConvertColorToChroma();
		var adjustedChroma = chroma;
		var chromaMultiplier = Mathf.Pow(2f, ChromaAdjustment);
		adjustedChroma.x *= chromaMultiplier;
		adjustedChroma.y *= chromaMultiplier;
		var brightnessMultiplier = Mathf.Pow(2f, BrightnessAdjustment);
		adjustedChroma.z *= brightnessMultiplier;
		AdjustedColor = adjustedChroma.ConvertChromaToColor();
		//clamp the color to ensure it is within valid range
		AdjustedColor.r = Mathf.Clamp01(AdjustedColor.r);
		AdjustedColor.g = Mathf.Clamp01(AdjustedColor.g);
		AdjustedColor.b = Mathf.Clamp01(AdjustedColor.b);
		AdjustedColor.a = 1.0f; // Ensure alpha is set to 1
		adjustedChroma = AdjustedColor.ConvertColorToChroma();
		
		Vector2 white = new Vector2(0f, 1f);
		Vector2 black = new Vector2(0f, 0f);
		Vector2 low = new Vector2(1f, 1/3f);
		Vector2 high = new Vector2(1f, 2f/3f);
		Handles.color = Color.white;
		Handles.DrawLine(white, black);
		Handles.DrawLine(low, high);
		Handles.DrawLine(white, high);
		Handles.DrawLine(black, low);

		Vector2 keyColorPosition = new Vector2(new Vector2(chroma.x, chroma.y).magnitude /(2/3f), chroma.z);
		Handles.color = KeyColor;
		Handles.SphereHandleCap(0, keyColorPosition, Quaternion.identity, 0.04f, EventType.Repaint);

		Vector2 adjustedColorPosition = new Vector2(new Vector2(adjustedChroma.x, adjustedChroma.y).magnitude /(2/3f), adjustedChroma.z);
		Handles.color = AdjustedColor;	
		Handles.SphereHandleCap(0, adjustedColorPosition, Quaternion.identity, 0.02f, EventType.Repaint);


		Handles.matrix = Matrix4x4.identity;
	}
#endif
}