#if UNITY_EDITOR
using UnityEditor.AssetImporters;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

#nullable enable
namespace ImageMath {
    [ScriptedImporter(1, "cube")]
    public class CubeImporter : ScriptedImporter {
        override public void OnImportAsset(AssetImportContext context) {
            var fileContent = System.IO.File.ReadAllText(context.assetPath);
            var data = FlatLUT3D.ParseLUT3D(fileContent, out int size, out Vector3 domainMin, out Vector3 domainMax, out string? title);
            var dimensions = FlatLUT3D.CalculateDimensions(size);
            var texture = Static.CreateTexture2D(dimensions.x, dimensions.y, GraphicsFormat.R32G32B32A32_SFloat, false);
            texture.SetPixelData(data, 0);
            texture.Apply();
            context.AddObjectToAsset("FlatLUT3D", texture);
            var matrixObject = ScriptableObject.CreateInstance<MatrixAsset>();
            matrixObject.matrix = Matrix4x4.TRS(domainMin, Quaternion.identity, domainMax - domainMin);
            context.AddObjectToAsset("MatrixData", matrixObject);
            context.SetMainObject(texture);
        }
    }
}
#endif