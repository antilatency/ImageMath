using UnityEngine;
using System.Linq;


#if UNITY_EDITOR
#endif
#nullable enable

namespace ImageMath.Views {
	[ExecuteAlways]
    public class FlatLUT3DView : MonoBehaviour {
		private const string GridShaderName = "ImageMath/Views/FlatLUT3DViewGrid";
		private const string PointsShaderName = "ImageMath/Views/FlatLUT3DViewPoints";

		public static void CheckMaterial(ref Material? material, string shaderName) {
			if (material == null) {
				var shader = Shader.Find(shaderName);
				if (shader == null) {
					Debug.LogError($"Shader not found: {shaderName}");
					return;
				}
				material = new Material(shader);
			}
		}

		private FlatLUT3DBase? CreatedLUT = null;
		public FlatLUT3DBase? LUT;

		public Material? _gridMaterial;
		public Material? _pointsMaterial;
		public Material? _shellMaterial;


		[Range(0f, 1f)]
		public float GridAlpha = 0.25f;
		[Range(0f, 1f)]
		public float ShellAlpha = 0.125f;
		[Range(0.00f, 1f)]
		public float PointSize = 0.125f;

		public FlatLUT3D ResizeLUT(int newSize) {
			if (CreatedLUT != null && CreatedLUT.Size == newSize && CreatedLUT is FlatLUT3D typed) {
				return typed;
			}

			CreatedLUT?.Dispose();
			var result = new FlatLUT3D(newSize);
			CreatedLUT = result;
			LUT = result;
			return result;
		}

		public FlatLUT3DRenderable ResizeLUTRenderable(int newSize) {
			if (CreatedLUT != null && CreatedLUT.Size == newSize && CreatedLUT is FlatLUT3DRenderable typed) {
				return typed;
			}
			CreatedLUT?.Dispose();
			var result = new FlatLUT3DRenderable(newSize);
			CreatedLUT = result;
			LUT = result;
			return result;
		}

		void Update() {
			if (LUT != CreatedLUT && LUT != null) {
				if (CreatedLUT) {
					CreatedLUT!.Dispose();
					CreatedLUT = null;
				}
			}

			CheckMaterial(ref _pointsMaterial, PointsShaderName);
			CheckMaterial(ref _gridMaterial, GridShaderName);
			CheckMaterial(ref _shellMaterial, GridShaderName);

			if (_pointsMaterial == null || _gridMaterial == null || _shellMaterial == null) {
				Debug.LogError("Materials not created.");
				return;
			}

			_pointsMaterial.SetFloat("PointSize", PointSize);
			_gridMaterial.SetFloat("Alpha", GridAlpha);
			_shellMaterial.SetFloat("Alpha", ShellAlpha);
			

			if (LUT != null) {
				ConfigureMaterial(_pointsMaterial);
				ConfigureMaterial(_gridMaterial);	
				ConfigureMaterial(_shellMaterial);

				var meshRenderer = GetComponent<MeshRenderer>();
				if (meshRenderer == null) {
					meshRenderer = gameObject.AddComponent<MeshRenderer>();
				}
				meshRenderer.sharedMaterials = new Material?[] {
					_pointsMaterial,
					_gridMaterial,
					_shellMaterial
				};
				var meshFilter = GetComponent<MeshFilter>();
				if (meshFilter == null) {
					meshFilter = gameObject.AddComponent<MeshFilter>();
				}
				var mesh = meshFilter.sharedMesh;
				if (mesh == null || mesh.name != LUT.Size.ToString()) {
					mesh = GenerateMesh(LUT.Size);
					mesh.name = LUT.Size.ToString();
					meshFilter.sharedMesh = mesh;
				}
			}
		}

		static Mesh GenerateMesh(int size) {
			var mesh = new Mesh();
			var vertices = new Vector3[size * size * size];
			int index = 0;
			for (int z = 0; z < size; z++) {
				for (int y = 0; y < size; y++) {
					for (int x = 0; x < size; x++) {
						vertices[index++] = new Vector3(
							(float)x / (size - 1),
							(float)y / (size - 1),
							(float)z / (size - 1)
						);
					}
				}
			}
			
			mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
			mesh.SetVertices(vertices);
			mesh.subMeshCount = 3;
			var indicesForPoints = Enumerable.Range(0, size * size * size).ToArray();
			mesh.SetIndices(indicesForPoints, MeshTopology.Points, 0, false);

			var indicesForGrid = GenerateGridIndices(size);
			mesh.SetIndices(indicesForGrid, MeshTopology.Lines, 1, false, 0);

			var indicesForShell = GenerateShellIndices(size);
			mesh.SetIndices(indicesForShell, MeshTopology.Triangles, 2, false, 0);

			mesh.RecalculateBounds();
			return mesh;
		}	


		static int[] GenerateShellIndices(int size) {
			var indices = new System.Collections.Generic.List<int>();
			//generate 6 surfaces of the cube
			void GenerateSurface(Vector3Int offset, Vector3Int uDir, Vector3Int vDir) { 
				for (int u = 0; u < size-1; u++) {
					for (int v = 0; v < size - 1; v++) {
						int x = offset.x + u * uDir.x + v * vDir.x;
						int y = offset.y + u * uDir.y + v * vDir.y;
						int z = offset.z + u * uDir.z + v * vDir.z;
						int index = x + y * size + z * size * size;
						int uOffset = uDir.x + uDir.y * size + uDir.z * size * size;
						int vOffset = vDir.x + vDir.y * size + vDir.z * size * size;
						// add 2 triangles
						indices.Add(index);
						indices.Add(index + vOffset);
						indices.Add(index + uOffset);
						
						indices.Add(index + uOffset);
						indices.Add(index + vOffset);
						indices.Add(index + uOffset + vOffset);
						
					}
				}
			}
			GenerateSurface(new Vector3Int(0, 0, 0), new Vector3Int(1, 0, 0), new Vector3Int(0, 1, 0)); // z-
			GenerateSurface(new Vector3Int(0, 0, 0), new Vector3Int(0, 0, 1), new Vector3Int(1, 0, 0)); // y-
			GenerateSurface(new Vector3Int(0, 0, 0), new Vector3Int(0, 1, 0), new Vector3Int(0, 0, 1)); // x-
			var max = new Vector3Int(size - 1, size - 1, size - 1);
			GenerateSurface(max, new Vector3Int(0, -1, 0), new Vector3Int(-1, 0, 0)); // z+
			GenerateSurface(max, new Vector3Int(-1, 0, 0), new Vector3Int(0, 0, -1)); // y+
			GenerateSurface(max, new Vector3Int(0, 0, -1), new Vector3Int(0, -1, 0)); // x+

			return indices.ToArray();
		}

		static int[] GenerateGridIndices(int size) {
			var indices = new System.Collections.Generic.List<int>();
			for (int z = 0; z < size; z++) {
				for (int y = 0; y < size; y++) {
					for (int x = 0; x < size; x++) {
						int index = x + y * size + z * size * size;
						// line to x+1
						if (x + 1 < size) {
							indices.Add(index);
							indices.Add(index + 1);
						}
						// line to y+1
						if (y + 1 < size) {
							indices.Add(index);
							indices.Add(index + size);
						}
						// line to z+1
						if (z + 1 < size) {
							indices.Add(index);
							indices.Add(index + size * size);
						}
					}
				}
			}
			return indices.ToArray();
		}


		void ConfigureMaterial(Material material) {
			int size = LUT!.Size;
			material.SetTexture("FlatLUT3D", LUT.Texture);
			material.SetInt("Size", size);
		}

		/*void OnRenderObject() {
			
			if (LUT == null) return;

			int size = LUT.Size;
			int numVertices = size * size * size;
			if (DrawPoints) {
				if (_pointsMaterial == null) {
					var shader = Shader.Find(PointsShaderName);
					if (shader == null) {
						Debug.LogError($"Shader not found: {PointsShaderName}");
						return;
					}
					_pointsMaterial = new Material(shader);
				}
				ConfigureMaterial(_pointsMaterial);

				
				_pointsMaterial.SetPass(0);
				
				Graphics.DrawProceduralNow(MeshTopology.Points, numVertices);
			}
			if (DrawGrid) {
				if (_gridMaterial == null) {
					var shader = Shader.Find(GridShaderName);
					if (shader == null) {
						Debug.LogError($"Shader not found: {GridShaderName}");
						return;
					}
					_gridMaterial = new Material(shader);
				}
				ConfigureMaterial(_gridMaterial);

				
				_gridMaterial.SetPass(0);

				Graphics.DrawProceduralNow(MeshTopology.Lines, numVertices);
			}

			/*if (_material == null) {
				var shader = Shader.Find(ShaderName);
				if (shader == null) {
					Debug.LogError($"Shader not found: {ShaderName}");
					return;
				}
				_material = new Material(shader);
			}
			
			_material.SetMatrix("ObjectToWorld", transform.localToWorldMatrix);
			_material.SetTexture("FlatLUT3D", LUT.Texture);
			int size = LUT.Size;
			_material.SetInt("Size", size);
			int numVertices = size * size * size;

			if (DrawGrid) {
				_material.SetFloat("Alpha", GridAlpha);
				_material.SetPass(1);
				Graphics.DrawProceduralNow(MeshTopology.Lines, numVertices);
			}
			if (DrawPoints) {
				_material.SetFloat("PointSize", PointSize);
				_material.SetPass(0);
				Graphics.DrawProceduralNow(MeshTopology.Points, numVertices);
			}

		}*/

		public static FlatLUT3DView GetByName(string name) {
			var found = FindObjectsOfType<FlatLUT3DView>(true).FirstOrDefault(x => x.name == name);
			if (found == null) {
				found = new GameObject(name).AddComponent<FlatLUT3DView>();
			}
			return found;
		}
	}
}
