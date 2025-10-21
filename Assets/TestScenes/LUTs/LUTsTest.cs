using System.Collections;
using System.Collections.Generic;
using System.IO;
using ImageMath;
using ImageMath.Views;
using UnityEngine;
#nullable enable
[ExecuteAlways]
public class LUTsTest : MonoBehaviour {
    [FilePathSelector("Select LUT3D File", "cube")]
    public string? LUT3DFilePath;

    public string? prewLUT3DFilePath = null;

    public FlatLUT3D? FlatLUT3D;

    public LUT3D? LUT3D;

    public Vector3 DomainMin = Vector3.zero;
    public Vector3 DomainMax = Vector3.one;

    public void Update(){

        if (prewLUT3DFilePath != LUT3DFilePath || FlatLUT3D == null || LUT3D == null) {

            if (!File.Exists(LUT3DFilePath)){
                return;
            }

            var data = File.ReadAllText(LUT3DFilePath);

            FlatLUT3D?.Dispose();
            FlatLUT3D = FlatLUT3D.CreateFromCubeFileContent(data, true);

            LUT3D?.Dispose();
            LUT3D = FlatLUT3D?.ToLUT3D();

            prewLUT3DFilePath = LUT3DFilePath;
        }

        if (FlatLUT3D == null) {
            return;
        }
        if (LUT3D == null) {
            return;
        }

        LUT3D.DomainMax = DomainMax;
        LUT3D.DomainMin = DomainMin;

        TextureView.GetByName("FlatLUT3D").Texture = FlatLUT3D.Texture;

        var lutTransformInput = TextureView.GetByName("LUT3DTransformInput").ResizeRenderTexture(1024,1024);
        new UVFill().AssignTo(lutTransformInput);

        var lutTransformOutput = TextureView.GetByName("LUT3DTransformOutput").ResizeRenderTexture(1024,1024);
        new LUT3DTransform(lutTransformInput, LUT3D).AssignTo(lutTransformOutput);
    }

}
