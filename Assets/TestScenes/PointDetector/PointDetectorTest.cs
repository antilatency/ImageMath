using System.Collections;
using System.Collections.Generic;
using System.IO;
using ImageMath;
using ImageMath.Views;
using UnityEngine;
#nullable enable
[ExecuteAlways]
public class PointDetectorTest : MonoBehaviour {

    public PointDetector.Segment[] Result;

    public void Update() {

        var input = TextureView.GetByName("Input").ResizeRenderTexture(32, 32);
        input.filterMode = FilterMode.Point;
        new EllipseFill(Vector4.one, Vector4.zero, null, Vector4.one * 0.25f).AssignTo(input);

        Result = new PointDetector(input,1).Execute();
    }

}
