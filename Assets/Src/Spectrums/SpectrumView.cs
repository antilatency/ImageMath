using System.Linq;
using ImageMath.ScriptableObjects;
using UnityEngine;
#nullable enable

namespace ImageMath.Views {
    [ExecuteAlways]
    [RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer))]
    public class SpectrumView : MonoBehaviour {
        public Color Color = Color.white;

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
                var gameObject = new GameObject(name);
                gameObject.transform.localScale = new Vector3(0.01f, 1, 1);
                found = gameObject.AddComponent<SpectrumView>();
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

            // Store raw spectrum data: x = wavelength (nm), y = intensity (0-1)
            Vector3[] points = new Vector3[spectrum.Values.Length];
            for (int i = 0; i < spectrum.Values.Length; i++) {
                float wavelength = spectrum.StartWavelength + i * spectrum.WavelengthStep;
                float intensity = spectrum.Values[i];
                points[i] = new Vector3(wavelength, intensity, 0);
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

        [ContextMenu("Save as SVG polyline")]
        private void SaveAsSVGPolyline() {
            SaveAsSVG(true);
        }
        
        [ContextMenu("Save as SVG filled")]
        private void SaveAsSVGFilled() {
            SaveAsSVG(false);
        }

        private void SaveAsSVG(bool lineOnly) {
            var spectrum = Spectrum;
            if (spectrum == null) {
                Debug.LogError("No spectrum to save");
                return;
            }
            string path = UnityEditor.EditorUtility.SaveFilePanel(
                "Save Spectrum As SVG",
                "",
                gameObject.name + ".svg",
                "svg"
            );
            if (string.IsNullOrEmpty(path)) {
                return;
            }

            var width = spectrum.Width;
            float height = 100;
            //format floats with invariant culture
            System.Globalization.CultureInfo invariantCulture = System.Globalization.CultureInfo.InvariantCulture;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine(@"<?xml version=""1.0"" standalone=""no""?>");
            sb.AppendLine(@"<!DOCTYPE svg PUBLIC ""-//W3C//DTD SVG 1.1//EN"" ""http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd"">");
            sb.AppendLine($@"<svg width=""{width}"" height=""{height}"" version=""1.1"" xmlns=""http://www.w3.org/2000/svg"" style=""background: #202020ff;"">");

            if (!lineOnly) {
                sb.Append($@"<polygon fill=""rgb({(int)(Color.r * 255)},{(int)(Color.g * 255)},{(int)(Color.b * 255)})"" stroke=""none"" points=""");
            } else {
                sb.Append($@"<polyline fill=""none"" stroke=""rgb({(int)(Color.r * 255)},{(int)(Color.g * 255)},{(int)(Color.b * 255)})"" stroke-width=""1"" points=""");
            }

            for (int i = 0; i < spectrum.Values.Length; i++) {
                float x = i * spectrum.WavelengthStep;
                float y = spectrum.Values[i];
                sb.Append($"{x},{height * (1 - y)} ");
            }

            if (!lineOnly) {
                //close the polygon
                sb.Append($"{width}, {height} 0, {height} ");
            }

            sb.AppendLine(@""" />");
            sb.AppendLine(@"</svg>");
            System.IO.File.WriteAllText(path, sb.ToString());
        }

#endif

    }
}
