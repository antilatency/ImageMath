using UnityEngine;
namespace ImageMath {
	public class DiscreteCosineTransform {
		public readonly int N;
		public readonly float iN;
		public readonly float m;
		public DiscreteCosineTransform(int n) {
			N = n;
			iN = 1f / n;
			m = Mathf.PI * iN;
		}

		public float Basis1D(int x, int u) {
			return Mathf.Cos((x + 0.5f) * u * m);
		}

		float DCTBasis(int x, int y, int u, int v) {
			/*return Mathf.Cos((x + 0.5f) * u * m) *
				   Mathf.Cos((y + 0.5f) * v * m);*/
			return Basis1D(x, u) * Basis1D(y, v);
		}


		public static float DCTBasis(int x, int y, int u, int v, int N) {
			float PI = Mathf.PI;
			return Mathf.Cos(((2.0f * x + 1.0f) * u * PI) / (2.0f * N)) *
				   Mathf.Cos(((2.0f * y + 1.0f) * v * PI) / (2.0f * N));
		}

		public float CalculateCoefficient(float[,] pixels, int u, int v) {
			float sum = 0f;
			for (int x = 0; x < N; x++) {
				for (int y = 0; y < N; y++) {
					sum += pixels[x, y] * DCTBasis(x, y, u, v);
				}
			}
			float au = u == 0 ? iN : 2f * iN;
			float av = v == 0 ? iN : 2f * iN;
			return sum * au * av;
		}

		public float[,] CalculateCoefficients(float[,] pixels) {
			float[,] coefficients = new float[N, N];

			for (int u = 0; u < N; u++) {
				for (int v = 0; v < N; v++) {

					coefficients[u, v] = CalculateCoefficient(pixels, u, v);

				}
			}
			return coefficients;
		}



		public float[,] ReconstructFromCoefficients(float[,] coefficients) {
			int n = coefficients.GetLength(0);
			float[,] pixels = new float[n, n];

			for (int x = 0; x < n; x++) {
				for (int y = 0; y < n; y++) {
					float sum = 0f;
					for (int u = 0; u < n; u++) {
						for (int v = 0; v < n; v++) {
							sum += coefficients[u, v] * DCTBasis(x, y, u, v);
						}
					}
					pixels[x, y] = sum;
				}
			}
			return pixels;
		}

	}
}