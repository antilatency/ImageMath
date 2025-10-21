using UnityEngine;
using ImageMath;
using ImageMath.Views;
using System.Linq;


#if UNITY_EDITOR
using UnityEditor;
#endif
#nullable enable

[ExecuteAlways]
public class LogTest : MonoBehaviour{

	public float DiffMultiplier = 1.0f;

	public float ExponentScale = 9.0f;
	[Range(0.0f, 0.15f)]
	public float BlackLevel = 0.0f;
	[Range(0.0f, 1.0f)]
	public float WhiteLevel = 1.0f;

	[Range(0.0f, 0.001f)]
	public float Slope = 0.1f;
	[Range(0.0f, 0.2f)]
	public float BreakPoint = 0.18f;

	public Vector3 TestValue = new Vector3(0.2f, 0.4f, 0.6f);
	public Vector3 Packed;
	public Vector3 Unpacked;

	float[] GetRow0R(Texture2D texture) {
		float[] row = new float[texture.width];
		var data = texture.GetRawTextureData<Vector4>();
		for (int x = 0; x < texture.width; x++) {
			row[x] = data[x].x;
		}
		return row;
	}

	float[] GetLinear(int width) {
		float[] gradient = new float[width];
		for (int x = 0; x < width; x++) {
			gradient[x] = (float)x / (width - 1);
		}
		return gradient;
	}

	float[] Transform(float[] input, ColorTransformOperation operation) {
		float[] transformed = new float[input.Length];
		for (int i = 0; i < input.Length; i++) {
			transformed[i] = operation.Convert(input[i]);
		}
		return transformed;
	}

	float[] Residuals(float[] original, float[] transformed) {
		float[] residuals = new float[original.Length];
		for (int i = 0; i < original.Length; i++) {
			/*if (original[i] < 0) {
				residuals[i] = 0;
				continue;
			}*/
			/*if (original[i] > 3) {
				residuals[i] = 0;
				continue;
			}*/
			residuals[i] = original[i] - transformed[i];
		}
		return residuals;
	}

	float[] Transform(float[] input, float[] parameters) {
		var operation = new UnpackLog {
			BlackLevel = parameters[0],
			ExponentScale = parameters[1],
			WhiteLevel = parameters[2],
			//Slope = parameters[3],
			//BreakPoint = parameters[4]
		};
		float[] transformed = new float[input.Length];
		for (int i = 0; i < input.Length; i++) {
			transformed[i] = operation.Convert(input[i]);
		}
		return transformed;
	}

	(double[] gradient, double[,] hessian) ComputeGradientAndHessian(float[] a, float[] b, float[] parameters, float delta = 0.0001f) {
		int paramCount = parameters.Length;
		double[] gradient = new double[paramCount];
		double[,] hessian = new double[paramCount, paramCount];

		var baseTransformed = Transform(b, parameters);
		var baseResiduals = Residuals(a, baseTransformed);

		double[][] derivatives = new double[paramCount][];
		/*double baseError = 0;
		for (int i = 0; i < baseResiduals.Length; i++) {
			baseError += baseResiduals[i] * baseResiduals[i];
		}*/
		//calc derivatives
		for (int p = 0; p < paramCount; p++) {
			var oldParamValue = parameters[p];
			parameters[p] += delta;
			var perturbedTransformed = Transform(b, parameters);
			var perturbedResiduals = Residuals(a, perturbedTransformed);
			derivatives[p] = new double[baseResiduals.Length];
			for (int i = 0; i < baseResiduals.Length; i++) {
				derivatives[p][i] = (perturbedResiduals[i] - baseResiduals[i]) / delta;
			}
			parameters[p] = oldParamValue;
		}
		//calc gradient
		for (int p = 0; p < paramCount; p++) {
			double grad = 0;
			for (int i = 0; i < baseResiduals.Length; i++) {
				grad += baseResiduals[i] * derivatives[p][i];
			}
			gradient[p] = grad;
		}
		//calc hessian
		for (int p = 0; p < paramCount; p++) {
			for (int q = 0; q < paramCount; q++) {
				double hess = 0;
				for (int i = 0; i < baseResiduals.Length; i++) {
					hess += derivatives[p][i] * derivatives[q][i];
				}
				hessian[p, q] = hess;
			}
		}
		return (gradient, hessian);
	}

	public InspectorButton _Optimize;
	public void Optimize() {

		var parameters = new float[] { BlackLevel, ExponentScale, WhiteLevel, Slope, BreakPoint };
		var bdfg5 = TextureView.GetByName("bdfg5").Texture;
		var a = GetRow0R((Texture2D)bdfg5);
		var b = GetLinear(bdfg5.width);

		var (gradient, hessian) = ComputeGradientAndHessian(a, b, parameters);
		Accord.Math.Decompositions.CholeskyDecomposition cholesky = new Accord.Math.Decompositions.CholeskyDecomposition(hessian);
		var deltas = cholesky.Solve(gradient);
		for (int i = 0; i < parameters.Length; i++) {
			parameters[i] -= (float)deltas[i];
		}
		BlackLevel = parameters[0];
		ExponentScale = parameters[1];
		WhiteLevel = parameters[2];
		Slope = parameters[3];
		BreakPoint = parameters[4];
		//make dirty
		EditorUtility.SetDirty(this);
	}


	public float[] Coefficients = new float[] { 0, 1 };


	void Update() {

		var bdfg5 = TextureView.GetByName("bdfg5").Texture;


		var linear = TextureView.GetByName("Gradient").ResizeRenderTexture(2048, 256);

		new GradientFill() {
			PointA = new Vector2(0.5f / 2048, 0),
			PointB = new Vector2(1 - 0.5f / 2048, 0),
		}.AssignTo(linear);


		var unpackOperation = TransferFunctions.UnpackBlackmagicDesignFilmGen5(linear);
		var packOperation = TransferFunctions.PackBlackmagicDesignFilmGen5();

		var a = GetRow0R((Texture2D)bdfg5);
		var b = GetLinear(bdfg5.width);
		var points = new Vector2[a.Length];
		for (int i = 0; i < points.Length; i++) {
			points[i] = new Vector2(b[i], a[i]);
		}




		/*unpackOperation.Solve(points, (a, b) => {
			Accord.Math.Decompositions.CholeskyDecomposition cholesky = new Accord.Math.Decompositions.CholeskyDecomposition(a);
			var result = cholesky.Solve(b);
			return result;
		});*/

		/*var packOperation = new PackLog {
			Texture = gradient,
			ExponentScale = ExponentScale,
			BlackLevel = BlackLevel,
			WhiteLevel = WhiteLevel,
		};*/

		var unpacked = TextureView.GetByName("Unpacked").ResizeRenderTexture(linear.width, linear.height);
		unpackOperation.AssignTo(unpacked);


		/*var log = TextureView.GetByName("Log").ResizeRenderTexture(256, 256);
		packOperation.AssignTo(log);




		var unpacked = TextureView.GetByName("Unpacked").ResizeRenderTexture(256, 256);

		unpackOperation.AssignTo(unpacked);*/

		var diff = TextureView.GetByName("Diff").ResizeRenderTexture(linear.width, linear.height);

		new AbsDiffOperation(bdfg5, unpacked, DiffMultiplier).AssignTo(diff);


		var equal = TextureView.GetByName("Equal").ResizeRenderTexture(linear.width, linear.height);
		new TextureCompare {
			Texture = diff,
			Operation = TextureCompare.CompareOperation.Equal,
			Reference = new Vector4(0.0f, 0.0f, 0.0f, 1.0f)
		}.AssignTo(equal);

		//Packed = packOperation.Convert(TestValue);
		//Unpacked = unpackOperation.Convert(Packed);

		var roundTrip = TextureView.GetByName("RoundTrip").ResizeRenderTexture(linear.width, linear.height);
		TransferFunctions.PackBlackmagicDesignFilmGen5(unpacked).AssignTo(roundTrip);

		var roundTripDiff = TextureView.GetByName("RoundTripDiff").ResizeRenderTexture(linear.width, linear.height);
		new AbsDiffOperation(linear, roundTrip, DiffMultiplier).AssignTo(roundTripDiff);

    }



#if UNITY_EDITOR

	void DrawFunctionGraph(float[] values) {
		var points = new Vector3[values.Length];
		for (int i = 0; i < values.Length; i++) {
			points[i] = new Vector3((float)i / (values.Length - 1), values[i], 0);
		}
		Handles.DrawPolyLine(points);
	}
	public void OnDrawGizmos() {
		Handles.matrix = transform.localToWorldMatrix;
		Handles.color = Color.green;
		var bdfg5 = TextureView.GetByName("bdfg5").Texture;
		if (bdfg5 != null) {
			var a = GetRow0R((Texture2D)bdfg5);
			DrawFunctionGraph(a);
		}
		//Handles.color = Color.red;
		var b = GetLinear(bdfg5.width);
		var unpackOperation = TransferFunctions.UnpackBlackmagicDesignFilmGen5();
		var transformed = Transform(b, unpackOperation);
		Handles.color = Color.red;
		DrawFunctionGraph(transformed);

		/*var unpackOperation = new UnpackLog {
			ExponentScale = ExponentScale,
			BlackLevel = BlackLevel,
			WhiteLevel = WhiteLevel,
		};
		var transformed = Transform(b, unpackOperation);
		DrawFunctionGraph(transformed);*/

		/*var linlog = new UnpackLinLog {
			ExponentScale = ExponentScale,
			BlackLevel = BlackLevel,
			WhiteLevel = WhiteLevel,
			Slope = Slope,
			BreakPoint = BreakPoint
		};
		var transformedLinLog = Transform(b, linlog);
		Handles.color = Color.blue;
		DrawFunctionGraph(transformedLinLog);*/


		Handles.matrix = Matrix4x4.identity;
	}
#endif
}