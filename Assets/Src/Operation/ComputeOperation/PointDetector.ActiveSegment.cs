#nullable enable
using System.Collections.Generic;

using UnityEngine;

namespace ImageMath {
    public partial record PointDetector {

        /*struct RawSegment {
            public float Start;
            public float Length;
            public float Sum;
            public float WeightedSum;
            public Vector4 ToVector4() {
                return new Vector4(Start, Length, Sum, WeightedSum);
            }
            public Segment ToGlobalSegment() {
                var globalWeightedSum = WeightedSum + Start * (double)Sum;
                return new Segment((int)Start, (int)Length, Sum, globalWeightedSum);
            }
        }*/




        public class ActiveSegment {
            public int Start;
            public int Length;
            public int PointIndex;
            public ActiveSegment(int start, int length, int pointIndex) {
                Start = start;
                Length = length;
                PointIndex = pointIndex;
            }
        }


        public struct RawPoint {
            public int PixelCount;
            public double SX;
            public double SY;
            public double SXX;
            public double SXY;
            public double SYY;

            //operator to add 2 poins
            public static RawPoint operator +(RawPoint a, RawPoint b) {
                return new RawPoint {
                    PixelCount = a.PixelCount + b.PixelCount,
                    SX = a.SX + b.SX,
                    SY = a.SY + b.SY,
                    SXX = a.SXX + b.SXX,
                    SXY = a.SXY + b.SXY,
                    SYY = a.SYY + b.SYY
                };
            }
/*
float ConvertToGlobalWeightedSquareSum(float sx2_local, float sx_local, float sum, float start)
{
    return sx2_local + 2f * start * sx_local + start * start * sum;
}
*/
            public void AddSegment(Segment segment, int y) {
                double globalSx = segment.sx + segment.start * (double)segment.s;
                double globalSxx = segment.sxx + 2 * segment.start * (double)segment.sx + segment.start * segment.start * (double)segment.s;
                PixelCount += segment.length;
                SX += globalSx;
                SY += y * segment.s;
                SXX += globalSxx;
                SXY += globalSx * y;
                SYY += y * y * segment.s;
            }                

            public static RawPoint operator +(RawPoint a, Segment b) {
                return new RawPoint {
                    PixelCount = a.PixelCount + b.length,
                    SX = a.SX + b.sx,
                };
            }

            public Vector2 Center => new Vector2((float)(SX / PixelCount), (float)(SY / PixelCount));
            public (Vector3 axisX, Vector3 axisY) GetEllipseAxes() {
                if (PixelCount == 0) {
                    return (Vector3.right, Vector3.up);
                }

                double meanX = SX / PixelCount;
                double meanY = SY / PixelCount;

                // Compute covariance matrix elements
                double covXX = SXX / PixelCount - meanX * meanX;
                double covXY = SXY / PixelCount - meanX * meanY;
                double covYY = SYY / PixelCount - meanY * meanY;

                // Symmetric 2x2 matrix: | a  b |
                //                       | b  c |
                double a = covXX, b = covXY, c = covYY;

                // Compute eigenvalues of the 2x2 matrix
                double trace = a + c;
                double det = a * c - b * b;
                double delta = System.Math.Sqrt(trace * trace - 4 * det);

                double lambda1 = 0.5 * (trace + delta); // major
                double lambda2 = 0.5 * (trace - delta); // minor

                // Compute eigenvectors
                Vector2 axis1, axis2;

                if (System.Math.Abs(b) > 1e-5) {
                    axis1 = new Vector2((float)(lambda1 - c), (float)b);
                    axis2 = new Vector2((float)(lambda2 - c), (float)b);
                }
                else {
                    axis1 = new Vector2(1, 0);
                    axis2 = new Vector2(0, 1);
                }

                axis1 = axis1.normalized;
                axis2 = axis2.normalized;

                return (
                    new Vector3(axis1.x, axis1.y, (float)System.Math.Sqrt(lambda1)),
                    new Vector3(axis2.x, axis2.y, (float)System.Math.Sqrt(lambda2))
                );
            }

        }
    }


    public static class PointDetectorSegmentExtensions {
        public static bool IntersectsWith(this PointDetector.ActiveSegment activeSegment, PointDetector.Segment segment) {
            return !(activeSegment.Start > segment.start + segment.length
            || activeSegment.Start + activeSegment.Length < segment.start);
        }

        public static void RemoveIndex(this IList<PointDetector.ActiveSegment> segments, int index) {
            foreach (var segment in segments) {
                if (segment.PointIndex >= index) {
                    segment.PointIndex--;
                }
            }
        }
              
    
    }
}
