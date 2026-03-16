using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;


public class Solver2DLines {
    // ABD = (A, B, D)
    public Vector3 ABD = Vector3.zero;
    // C = (Cx, Cy)
    public Vector2 C = Vector2.zero;

    /// <summary>
    /// line.xy = normal (a, b)
    /// line.z  = c
    /// line.w  = weight
    /// </summary>
    public void AddLine(Vector4 line) {
        float a = line.x;
        float b = line.y;
        float c = line.z;
        float w = line.w;

        ABD.x += w * a * a;   // A
        ABD.y += w * a * b;   // B
        ABD.z += w * b * b;   // D

        C.x += w * a * c;   // Cx
        C.y += w * b * c;   // Cy
    }

    /// <summary>
    /// Solve weighted least-squares line intersection.
    /// </summary>
    public Vector2 Solve() {
        float A = ABD.x;
        float B = ABD.y;
        float D = ABD.z;
        float Cx = C.x;
        float Cy = C.y;

        float det = A * D - B * B;
        if (Mathf.Abs(det) < 1e-8f) {
            // Degenerate system: nearly parallel lines
            return Vector2.zero;
        }

        float x = (B * Cy - D * Cx) / det;
        float y = (B * Cx - A * Cy) / det;

        return new Vector2(x, y);
    }

    public void AddVerticalLine(float x) {
        /*float a = 1;
        float b = 0;
        float c = -x;*/

        ABD.x += 1;   // A
        //ABD.y += a * b;   // B
        //ABD.z += b * b;   // D

        C.x += -x;   // Cx
        //C.y += b * c;   // Cy
    }

    public void AddDiagonalLine(float x0, float x1) {
        // line from (x0,0) to (x1,1)

        float vx = x1 - x0;
        //float vy = 1;
        var svx = vx * vx;
        var sl = svx + 1;
        var isl = 1 / sl;
        //var l = Mathf.Sqrt(sl);

        //float a = -1 / l;
        //float b = vx / l;
        //float c = x0 / l;

        ABD += new Vector3(1, -vx, svx) * isl;

        /*ABD.x += isl;   // A
        ABD.y += -vx * isl;   // B
        ABD.z += svx * isl;   // D*/

        C += new Vector2(-x0, vx * x0) * isl;
        /*
        C.x += -x0 * isl;   // Cx
        C.y += vx * x0 * isl;   // Cy*/
    }

    public void AddHorizontalLine(float y, float weight) {
        /*float a = 0;
        float b = 1;
        float c = -y;*/

        ABD.z += weight;   // D
        //ABD.x += a * a * weight;   // A
        //ABD.y += a * b * weight;   // B

        C.y += -y * weight;   // Cy
        //C.x += a * c * weight;   // Cx
    }


    public float AddAllAndSolve(float a, float b, float o, float w) {
        AddVerticalLine(o);
        AddDiagonalLine(a, b);
        AddHorizontalLine(0.5f, w);
        return Solve().y;
    }

    public static float Static(float a, float b, float o, float w) {
        float A = 0;
        float B = 0;
        float D = 0;
        float Cx = 0;
        float Cy = 0;
        // Vertical line x = o
        A += 1;
        Cx += -o;
        // Diagonal line from (a,0) to (b,1)
        {
            float vx = b - a;
            var svx = vx * vx;
            var sl = svx + 1;
            var isl = 1 / sl;

            A += isl;
            B += -vx * isl;
            D += svx * isl;

            Cx += -a * isl;
            Cy += vx * a * isl;
        }
        // Horizontal line y = 0.5 with weight w
        D += w;
        Cy += -0.5f * w;

        float det = A * D - B * B;
        if (Mathf.Abs(det) < 1e-8f) {
            // Degenerate system: nearly parallel lines
            return 0.5f;
        }

        float y = (B * Cx - A * Cy) / det;

        return y;
    }

    public static float GradientInterpolator(float left, float right, float center, float w) {
        float vx = right - left;
        float vx2 = vx * vx;
        float hw = 0.5f * w;
        float sl = vx2 + 1f;
        float isl = 1f / sl;

        float num = hw + isl * (vx * (center - left) + hw);
        float det = w + (w + vx2) * isl;
        
        return num / det;
    }

}


public class LineCrossVisualization : MonoBehaviour {

    public float A = 0;
    public float B = 1;
    public float O = 0.5f;

    [Range(0.0f, 1f)]
    public float W = 1;

#if UNITY_EDITOR

    void OnDrawGizmos() {
        Handles.matrix = transform.localToWorldMatrix;

        //unit rect
        Handles.color = Color.white;
        Handles.DrawSolidRectangleWithOutline(new Rect(A, 0, B - A, 1), new Color(1, 1, 1, 0.1f), Color.white);

        var solver = new Solver2DLines();

        //add vertical line x=O
        //solver.AddLine(new Vector4(1, 0, -O, 1));
        solver.AddVerticalLine(O);

        Handles.color = Color.yellow;
        Handles.DrawLine(new Vector3(O, 0, 0), new Vector3(O, 1, 0));

        //add diagonal line from (A,0) to (B,1)
        /*var direction = new Vector2(B - A, 1 - 0);//.normalized;
        var normal = new Vector2(-direction.y, direction.x);
        solver.AddLine(new Vector4(normal.x, normal.y, -(normal.x * A + normal.y * 0), 1));*/

        solver.AddDiagonalLine(A, B);

        Handles.color = Color.cyan;
        Handles.DrawLine(new Vector3(A, 0, 0), new Vector3(B, 1, 0));

        //add horizontal line y=0.5 with weight W
        //solver.AddLine(new Vector4(0, 1, -0.5f, W));
    
        solver.AddHorizontalLine(0.5f, W);

        Handles.color = Color.magenta;
        Handles.DrawLine(new Vector3(A, 0.5f, 0), new Vector3(B, 0.5f, 0));


        var intersection = solver.Solve();
        Handles.color = Color.red;
        Handles.DrawSolidDisc(new Vector3(intersection.x, intersection.y, 0), Vector3.forward, 0.02f);


        var t = Solver2DLines.GradientInterpolator(A, B, O, W);
        Handles.color = Color.red;
        Handles.DrawLine(new Vector3(A, t, 0), new Vector3(B, t, 0));

        Handles.matrix = Matrix4x4.identity;
    }
#endif
}
