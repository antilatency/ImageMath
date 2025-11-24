using System.Linq;

using ImageMath.ScriptableObjects;

using UnityEngine;

namespace ImageMath.Views {
    [ExecuteAlways]
    [RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer))]
    public class SpectrumView : MonoBehaviour {
        public Color Color = Color.white;
        public float StartWavelength = 380;
        public float UnitsPerNanometer = 0.01f;
        public float HeightScale = 1.0f;

        public ScriptableSpectrum? SpectrumAsset;
        public Spectrum? _spectrum;
        public Spectrum? Spectrum {
            get {
                if (SpectrumAsset != null) {
                    return SpectrumAsset.Value;
                }
                return _spectrum;
            }
            set {
                _spectrum = value;
                SpectrumAsset = null;
                UpdateMesh();
            }
        }

        public static SpectrumView GetByName(string name) {
            var found = GameObject.FindObjectsOfType<SpectrumView>(true).FirstOrDefault(x => x.name == name);
            if (found == null) {
                found = new GameObject(name).AddComponent<SpectrumView>();
            }
            return found;
        }

        private Spectrum? spectrumOfCurrentMesh = null;
        public void Update() {
            UpdateMesh();
            UpdateMeshRenderer();
        }

        void OnValidate() {
            UpdateMesh();
            UpdateMeshRenderer();
        }

        private void UpdateMesh() {
            var spectrum = Spectrum;
            if (spectrum == null) {
                return;
            }
            if (spectrumOfCurrentMesh != spectrum) {
                RebuildMesh();
                spectrumOfCurrentMesh = new Spectrum(spectrum);
            }
        }

        private void UpdateMeshRenderer() {
            var meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer == null) {
                meshRenderer = gameObject.AddComponent<MeshRenderer>();
            }

            var material = meshRenderer.sharedMaterial;
            if (material == null) {
                var shader = Shader.Find("ImageMath/Views/SpectrumView");
                material = new Material(shader);
                meshRenderer.sharedMaterial = material;
            }

            material.color = Color;
        }

        private void RebuildMesh() {
            var spectrum = Spectrum;
            if (spectrum == null) {
                return;
            }
            var meshFilter = GetComponent<MeshFilter>();
            if (meshFilter == null) {
                meshFilter = gameObject.AddComponent<MeshFilter>();
            }
            var mesh = new Mesh();
            meshFilter.mesh = mesh;

            Vector3[] points = new Vector3[spectrum.Values.Length];
            var iScale = spectrum.WavelengthStep * UnitsPerNanometer;
            for (int i = 0; i < spectrum.Values.Length; i++) {
                float x = (spectrum.StartWavelength + i * spectrum.WavelengthStep - StartWavelength) * UnitsPerNanometer;
                float y = spectrum.Values[i] * HeightScale;
                points[i] = new Vector3(x, y, 0);
            }

            mesh.vertices = points;
            int[] indices = new int[(spectrum.Values.Length - 1) * 2];
            for (int i = 0; i < spectrum.Values.Length - 1; i++) {
                indices[i * 2] = i;
                indices[i * 2 + 1] = i + 1;
            }
            mesh.SetIndices(indices, MeshTopology.Lines, 0);
        }

#if UNITY_EDITOR
        [ContextMenu("Save as Spectrum")]
        private void SaveAsSpectrum() {
            var spectrum = Spectrum;
            if (spectrum == null) {
                Debug.LogError("No spectrum to save");
                return;
            }
            string path = UnityEditor.EditorUtility.SaveFilePanel(
                $"Save Image As {Spectrum.FileExtension.ToUpper()}",
                "",
                gameObject.name,
                Spectrum.FileExtension
            );
            if (string.IsNullOrEmpty(path)) {
                return;
            }
            var json = spectrum.ToJson();
            System.IO.File.WriteAllText(path, json);
        }
#endif

    }
}
