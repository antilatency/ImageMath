#if UNITY_EDITOR
using System;

using UnityEditor.AssetImporters;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

#nullable enable
namespace ImageMath {

    /*public static class RawTextureImporterFormats { 
        public const string[] GetFormats =
            new string[] {
                "i8", "i8i8i8i8",
                "f32", "f32f32f32f32"
            };
        
    }*/

    [ScriptedImporter(1, "raw")]
    public class RawTextureImporter : ScriptedImporter {


        public (GraphicsFormat Format, int PixelSize) GetFormatAndPixelSize(string pixelFormat) {
            return pixelFormat switch {
                "i8" => (GraphicsFormat.R8_UNorm, 1),
                "i8i8i8i8" => (GraphicsFormat.R8G8B8A8_UNorm, 4),
                "f32" => (GraphicsFormat.R32_SFloat, 4),
                "f32f32f32f32" => (GraphicsFormat.R32G32B32A32_SFloat, 16),
                _ => throw new System.Exception($"Unsupported pixel format: {pixelFormat}"),
            };
        }

        override public void OnImportAsset(AssetImportContext context) {
            var fileName = System.IO.Path.GetFileName(context.assetPath);
            var parts = fileName.Split('.');
            if (parts.Length < 3) {
                Debug.LogError($"No secondaryExtension: {fileName}");
                return;
            }
            var secondaryExtension = parts[parts.Length - 2];


            var regex = new System.Text.RegularExpressions.Regex(@"(\d+)(.*)");
            var match = regex.Match(secondaryExtension);
            if (!match.Success) {
                Debug.LogError($"Invalid raw texture secondaryExtension: {secondaryExtension}");
                return;
            }
            int width = int.Parse(match.Groups[1].Value);
            var graphicsFormatString = match.Groups[2].Value.ToLowerInvariant();

            //find enum value from GraphicsFormat
            var names = Enum.GetNames(typeof(GraphicsFormat));
            var graphicsFormat = GraphicsFormat.None;
            for (int i = 0; i < names.Length; i++) {
                var name = names[i].ToLowerInvariant();
                if (name == graphicsFormatString) {
                    graphicsFormat = ((GraphicsFormat[])Enum.GetValues(typeof(GraphicsFormat)))[i];
                    break;
                }
            }
            if (graphicsFormat == GraphicsFormat.None) {
                Debug.LogError($"Unsupported graphics format: {graphicsFormatString}");
                return;
            }
            var lineSize = GraphicsFormatUtility.ComputeMipmapSize(width, 1, graphicsFormat);
            var content = System.IO.File.ReadAllBytes(context.assetPath);
            int height = content.Length / (int)lineSize;
            var texture = Static.CreateTexture2D(width, height, graphicsFormat, false);
            texture.SetPixelData(content, 0);
            texture.Apply();
            context.AddObjectToAsset("RawTexture", texture);
            context.SetMainObject(texture);

        }
    }

}
#endif