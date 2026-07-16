#nullable enable

using System;

using UnityEngine;

namespace ImageMath {

    public struct RGBColorPrimaries {
        public Vector2 r, g, b;
    }

    public struct RGBColorGamut {
        public RGBColorPrimaries primaries;
        public Vector2 whitePoint;

        public RGBColorGamut(RGBColorPrimaries primaries, Vector2 whitePoint) {
            this.primaries = primaries;
            this.whitePoint = whitePoint;
        }

        public RGBColorGamut(Vector2 r, Vector2 g, Vector2 b, Vector2 whitePoint) {
            this.primaries = new RGBColorPrimaries { r = r, g = g, b = b };
            this.whitePoint = whitePoint;
        }

        public enum TransformDirection {
            XYZFromRGB,
            RGBFromXYZ,
        }

        public Matrix4x4 CalcTransformMatrix(TransformDirection direction) {
            var r = primaries.r;
            var g = primaries.g;
            var b = primaries.b;
            var w = whitePoint;

            // Some sources insist on normalizing the columns to Y=1, but
            // this is not necessary for the end result to be correct.
            var m = new Matrix4x4();
            m[0, 0] = r.x;
            m[1, 0] = r.y;
            m[2, 0] = 1 - (r.x + r.y);

            m[0, 1] = g.x;
            m[1, 1] = g.y;
            m[2, 1] = 1 - (g.x + g.y);

            m[0, 2] = b.x;
            m[1, 2] = b.y;
            m[2, 2] = 1 - (b.x + b.y);

            m[3, 3] = 1;

            // But that normalization is necessary for the white point.
            var wp = new Vector3();
            wp[0] = w.x / w.y;
            wp[1] = 1;
            wp[2] = (1 - (w.x + w.y)) / w.y;

            // TODO: Use QR decomposition (or Gaussian elimination) to avoid direct matrix inversion.
            Matrix4x4 mInv = m.inverse;
            var scale = mInv * wp;

            switch (direction) {
                case TransformDirection.XYZFromRGB:
                    for (int colIdx = 0; colIdx < 3; colIdx++) {
                        var s = scale[colIdx];
                        m[0, colIdx] *= s;
                        m[1, colIdx] *= s;
                        m[2, colIdx] *= s;
                    }

                    return m;

                case TransformDirection.RGBFromXYZ:
                    for (int rowIdx = 0; rowIdx < 3; rowIdx++) {
                        var s = 1.0f / scale[rowIdx];
                        mInv[rowIdx, 0] *= s;
                        mInv[rowIdx, 1] *= s;
                        mInv[rowIdx, 2] *= s;
                    }

                    return mInv;

                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }
    }
}
