using UnityEngine;
using UnityEditor;
using System.Linq;
using ImageMath;


[ExecuteAlways]
public class DiscreteCosineTransformTest : MonoBehaviour
{
    [Range(0, 7)]
    public int u = 0;

    [Range(0, 7)]
    public int v = 0;

    //public float cellSize = 0.5f;
    public float heightScale = 0.5f;

    public int N = 8;

	float[,] heights;





	public Vector3[] TestData = new Vector3[3];
	float[,] testDataPixels => new float[3, 3] {
		{ TestData[0].x, TestData[0].y, TestData[0].z },
		{ TestData[1].x, TestData[1].y, TestData[1].z },
		{ TestData[2].x, TestData[2].y, TestData[2].z }
	};

	public Vector3[] Coefficients = new Vector3[3];
	public Vector3[] Reconstruction = new Vector3[3];

	Vector3[] PackToV3(float[,] coefficients) {
		//coefficients is 3x3
		Vector3[] packed = new Vector3[3] {
			new Vector3(coefficients[0, 0], coefficients[0, 1], coefficients[0, 2]),
			new Vector3(coefficients[1, 0], coefficients[1, 1], coefficients[1, 2]),
			new Vector3(coefficients[2, 0], coefficients[2, 1], coefficients[2, 2])
		};
		return packed;
	}

	float[,] UnpackFromV3(Vector3[] packed) {
		//packed is 3x3
		float[,] coefficients = new float[3, 3] {
			{ packed[0].x, packed[0].y, packed[0].z },
			{ packed[1].x, packed[1].y, packed[1].z },
			{ packed[2].x, packed[2].y, packed[2].z }
		};
		return coefficients;
	}


	public bool CalculateCoefficients = true;

#if UNITY_EDITOR
	private void OnDrawGizmos() {

		var dct = new DiscreteCosineTransform(3);
		float[,] c;
		if (CalculateCoefficients) {
			c = dct.CalculateCoefficients(testDataPixels);
			Coefficients = PackToV3(c);
		} else {
			c = UnpackFromV3(Coefficients);
		}


		var r = dct.ReconstructFromCoefficients(c);
		Reconstruction = PackToV3(r);



		float cellSize = 1f / N;
		heights = new float[N, N];
		Handles.matrix = transform.localToWorldMatrix;

		// Step 1: Compute height values
		for (int y = 0; y < N; y++) {
			for (int x = 0; x < N; x++) {
				//normalized coordinates in range [-1, 1]
				float nx = (x / (float)(N - 1)) * 2.0f - 1.0f;
				float ny = (y / (float)(N - 1)) * 2.0f - 1.0f;
				float val = DiscreteCosineTransform.DCTBasis(x, y, u, v, N);
				//var val = DCTBasisForNormalizedXY(nx, ny, u, v);
				heights[x, y] = val * heightScale;
			}
		}

		// Step 2: Draw rows (horizontal lines)
		for (int y = 0; y < N; y++) {
			Vector3[] rowPoints = new Vector3[N];
			for (int x = 0; x < N; x++) {
				rowPoints[x] = new Vector3(x * cellSize, y * cellSize, heights[x, y]);
			}
			Handles.DrawPolyLine(rowPoints);
		}

		// Step 3: Draw columns (vertical lines)
		for (int x = 0; x < N; x++) {
			Vector3[] colPoints = new Vector3[N];
			for (int y = 0; y < N; y++) {
				colPoints[y] = new Vector3(x * cellSize, y * cellSize, heights[x, y]);
			}
			Handles.DrawPolyLine(colPoints);
		}

		Handles.matrix = Matrix4x4.identity;
	}
#endif
}