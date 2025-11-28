#if UNITY_EDITOR
using System.Collections.Generic;

using ImageMath.ScriptableObjects;
using UnityEngine;

# nullable enable
namespace ImageMath {
    [UnityEditor.AssetImporters.ScriptedImporter(1, "spectrum")]
    public class SpectrumImporter : UnityEditor.AssetImporters.ScriptedImporter {
        public override void OnImportAsset(UnityEditor.AssetImporters.AssetImportContext ctx) {
            var json = new TextAsset(System.IO.File.ReadAllText(ctx.assetPath));
            Spectrum? spectrum = Newtonsoft.Json.JsonConvert.DeserializeObject<Spectrum>(json.text);
            if (spectrum == null) {
                Debug.LogError($"Failed to deserialize spectrum from {ctx.assetPath}");
                return;
            }
            var scriptableSpectrum = ScriptableObject.CreateInstance<ScriptableSpectrum>();
            scriptableSpectrum.Value = spectrum;
            ctx.AddObjectToAsset("spectrum", scriptableSpectrum);
            ctx.SetMainObject(scriptableSpectrum);
        }

    }

    [UnityEditor.AssetImporters.ScriptedImporter(1, "hopoocolor")]
    public class SpectrumImporterHopooColor : UnityEditor.AssetImporters.ScriptedImporter {         

        public override void OnImportAsset(UnityEditor.AssetImporters.AssetImportContext ctx) {

            var csvData = System.IO.File.ReadAllLines(ctx.assetPath);
            List<float> values = new List<float>();
            float startWavelength = 0f;
            for (int i = 0; i < csvData.Length; i++) {
                var line = csvData[i];
                if (string.IsNullOrWhiteSpace(line)) {
                    continue;
                }
                var parts = line.Split(',');
                if (parts.Length != 3) continue;

                if (float.TryParse(parts[0], out float wavelength)) {
                    if (values.Count == 0) {
                        startWavelength = wavelength;
                    }
                    var value = float.Parse(parts[1]);
                    values.Add(value);
                }
            }

            var spectrum = new Spectrum(values, startWavelength, 1f);
            var scriptableSpectrum = ScriptableObject.CreateInstance<ScriptableSpectrum>();
            scriptableSpectrum.Value = spectrum;
            ctx.AddObjectToAsset("spectrum", scriptableSpectrum);
            ctx.SetMainObject(scriptableSpectrum);
        }

    }


}
#endif