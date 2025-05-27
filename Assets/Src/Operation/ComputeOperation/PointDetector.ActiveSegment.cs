#nullable enable
using System;
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

        [System.Serializable]
        public class RawPoint {
            public int PixelCount;
            public double S;
            public double SX;
            public double SY;
            public double SXX;
            public double SXY;
            public double SYY;

            //operator to add 2 poins
            public static RawPoint operator +(RawPoint a, RawPoint b) {
                return new RawPoint {
                    PixelCount = a.PixelCount + b.PixelCount,
                    S = a.S + b.S,
                    SX = a.SX + b.SX,
                    SY = a.SY + b.SY,
                    SXX = a.SXX + b.SXX,
                    SXY = a.SXY + b.SXY,
                    SYY = a.SYY + b.SYY
                };
            }

            public void AddSegment(Segment segment, int y) {
                double globalSx = segment.sx + segment.start * (double)segment.s;
                double globalSxx = segment.sxx + 2 * segment.start * (double)segment.sx + segment.start * segment.start * (double)segment.s;
                PixelCount += segment.length;
                S += segment.s;
                SX += globalSx;
                SY += y * segment.s;
                SXX += globalSxx;
                SXY += globalSx * y;
                SYY += y * y * segment.s;
            }                

            public Vector2 Center => new Vector2((float)(SX / S), (float)(SY / S));
            
            public (Vector3 axisX, Vector3 axisY) GetEllipseAxes() {
                if (S == 0) {
                    return (Vector3.right, Vector3.up);
                }

                double meanX = SX / S;
                double meanY = SY / S;

                double covXX = SXX / S - meanX * meanX;
                double covXY = SXY / S - meanX * meanY;
                double covYY = SYY / S - meanY * meanY;



                // Eigenvalue decomposition of 2x2 symmetric matrix
                double trace = covXX + covYY;
                double det = covXX * covYY - covXY * covXY;
                double sqrtTerm = Math.Sqrt(Math.Max(0, trace * trace / 4 - det));

                double lambda1 = trace / 2 + sqrtTerm;
                double lambda2 = trace / 2 - sqrtTerm;


                double dir1x,dir1y, dir2x, dir2y;
                if (Math.Abs(covXY) > 1e-10) {
                    // Calculate the direction vectors based on the eigenvalues and covariances
                    dir1x = (float)(lambda1 - covYY);
                    dir1y = (float)covXY;
                    dir2x = (float)(lambda2 - covYY);
                    dir2y = (float)covXY;
                    // Normalize the direction vectors
                    double length1 = Math.Sqrt(dir1x * dir1x + dir1y * dir1y);
                    double length2 = Math.Sqrt(dir2x * dir2x + dir2y * dir2y);
                    dir1x /= length1;
                    dir1y /= length1;
                    dir2x /= length2;
                    dir2y /= length2;
                } else {
                    // If covXY is zero, the covariance matrix is diagonal
                    dir1x = covXX >= covYY ? 1 : 0;
                    dir1y = covXX >= covYY ? 0 : 1;
                    dir2x = -dir1y; // orthogonal
                    dir2y = dir1x; // orthogonal
                }

                float len1 = (float)Math.Sqrt(Math.Max(0, lambda1));
                float len2 = (float)Math.Sqrt(Math.Max(0, lambda2));

                Vector3 axisX = new Vector3((float)dir1x, (float)dir1y, 2*len1);
                Vector3 axisY = new Vector3((float)dir2x, (float)dir2y, 2*len2);

                return (axisX, axisY);
            }

        }
    }


    public static class PointDetectorSegmentExtensions {
        public static bool IntersectsWith(this PointDetector.ActiveSegment activeSegment, PointDetector.Segment segment) {
            if (activeSegment.Start + activeSegment.Length <= segment.start)
                return false;
            if (activeSegment.Start >= segment.start + segment.length)
                return false;
            return true;
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
