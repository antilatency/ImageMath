using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ImageMath;
using ImageMath.Views;
#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
#nullable enable
[ExecuteAlways]
public class PointDetectorTest : MonoBehaviour {
    public int MaxSegmentsInRow = 2;
    private PointDetector.Segment[]? Segments;
    public PointDetector.RawPoint[]? Points;


    public void Update() {

        
        var input = TextureView.GetByName("Input");
        if (input == null || input.Texture == null) {
            Debug.LogError("Input texture is null");
            return;
        }

        for (int i = 0; i < 1; i++) {

            var detector = new PointDetector(input.Texture, MaxSegmentsInRow);
            Segments = detector.GetSegments();

            Points = detector.GetPoints(Segments, out var maxSegmentsInRowExceeded).ToArray();
            if (maxSegmentsInRowExceeded) {
                Debug.LogError("Max segments in row exceeded: " + MaxSegmentsInRow);
            }

        }

        //Result = new PointDetector(TextureView.GetByName("Input").Texture, 1).Execute();
    }

    public bool ShowSegments = false;

    public void OnDrawGizmos() {
        if (Segments == null || Points == null) return;
        var texture = TextureView.GetByName("Input")?.Texture;
        if (texture == null) {
            Debug.LogError("Input texture is null");
            return;
        }

        //set pixel scale
        
        Handles.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1f / texture.height, 1f / texture.height, 1f / texture.height));
        Gizmos.matrix = Handles.matrix;

        if (ShowSegments) {
            Handles.color = Color.red;
            for (int i = 0; i < Segments.Length; i++) {
                if (Segments[i].length <= 0) continue;
                var segment = Segments[i];
                var y = i % texture.height;
                Handles.DrawLine(new Vector3(segment.start + 0.5f, y + 0.5f), new Vector3(segment.start + segment.length - 0.5f, y + 0.5f));
            }
        }

        Handles.color = Color.green;
        Gizmos.color = Handles.color;
        foreach (var point in Points) {
            var center = point.Center + Vector2.one * 0.5f;
            var ellipse = point.GetEllipseAxes();
            Gizmos.DrawSphere(center, 0.1f * Mathf.Max(ellipse.axisX.z, ellipse.axisY.z));
            //draw ellipse
            var points = Enumerable.Range(0, 361).Select(a => {
                var angle = a * Mathf.Deg2Rad;
                var point = center + new Vector2(ellipse.axisX.x, ellipse.axisX.y) * ellipse.axisX.z * Mathf.Cos(angle) +
                             new Vector2(ellipse.axisY.x, ellipse.axisY.y) * ellipse.axisY.z * Mathf.Sin(angle);
                return new Vector3(point.x, point.y, 0);
            }).ToArray();
            Handles.DrawPolyLine(points);
        }
    }   

}
