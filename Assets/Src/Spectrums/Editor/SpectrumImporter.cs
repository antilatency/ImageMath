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
}
