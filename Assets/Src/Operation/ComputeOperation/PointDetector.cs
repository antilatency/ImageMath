#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

using UnityEngine;

namespace ImageMath {



    [FilePath]
    public partial record PointDetector : ComputeOperation {
        public int MaxSegmentsInRow { get; set; } = 32;
        public Texture Texture { get; set; }

        public PointDetector(Texture texture, int maxPointsInRow = 32) {
            MaxSegmentsInRow = maxPointsInRow;
            Texture = texture;
        }

        protected override Vector2Int GetDispatchSize() {
            if (Texture == null) {
                throw new System.Exception("Texture is null");
            }
            //var width = Texture.width;
            var height = Texture.height;

            var computeShader = ComputeShader;
            computeShader.GetKernelThreadGroupSizes(0, out var threadGroupSizeX, out var threadGroupSizeY, out var threadGroupSizeZ);

            int groupsX = 1; //Mathf.CeilToInt(width / (float)threadGroupSizeX);
            int groupsY = Mathf.CeilToInt(height / (float)threadGroupSizeY);
            return new Vector2Int(groupsX, groupsY);
        }




        public override ComputeBufferParameters GetComputeBufferParameters() {
            var bufferSize = Texture.height * MaxSegmentsInRow;
            return new ComputeBufferParameters(bufferSize, GetStride(), ComputeBufferType.Default, ComputeBufferMode.Immutable);
        }

        public Segment[] GetSegments() {
            
            var segments = Execute<Segment>();

            return segments;
        }

        
            


        public List<RawPoint> GetPoints(Segment[] segments, out bool maxSegmantsInRowExceeded) {


            List<RawPoint> points = new();

            List<ActiveSegment> activeSegments = new(MaxSegmentsInRow);
            List<ActiveSegment> newActiveSegments = new(MaxSegmentsInRow);
            maxSegmantsInRowExceeded = false;

            for (int y = 0; y < Texture.height; y++) {

                for (int x = 0; x < MaxSegmentsInRow; x++) {
                    var segmentIndex = y + x * Texture.height;
                    var segment = segments[segmentIndex];
                    if (segment.length == 0) {
                        if (segment.start == ushort.MaxValue) {
                            maxSegmantsInRowExceeded = true;
                        }                            
                        break;
                    }
                    int pointIndex = -1;
                    for (int i = 0; i < activeSegments.Count; i++) {
                        var activeSegment = activeSegments[i];
                        if (activeSegment.IntersectsWith(segment)) {
                            if (pointIndex >= 0) { //not the first active segment that intersects, merge points
                                if (pointIndex != activeSegment.PointIndex) {
                                    int minPointIndex;
                                    int maxPointIndex;
                                    if (pointIndex < activeSegment.PointIndex) {
                                        minPointIndex = pointIndex;
                                        maxPointIndex = activeSegment.PointIndex;
                                    }
                                    else {
                                        minPointIndex = activeSegment.PointIndex;
                                        maxPointIndex = pointIndex;
                                    }
                                    points[minPointIndex] += points[maxPointIndex];
                                    points.RemoveAt(maxPointIndex);
                                    activeSegments.RemoveIndex(maxPointIndex);
                                    pointIndex = minPointIndex;
                                }

                            }
                            else {
                                pointIndex = activeSegment.PointIndex;
                            }
                        }
                    }

                    if (pointIndex < 0) {
                        pointIndex = points.Count;
                        var newPoint = new RawPoint();
                        newPoint.AddSegment(segment, y);
                        points.Add(newPoint);
                    }
                    else {
                        points[pointIndex].AddSegment(segment, y);
                    }

                    newActiveSegments.Add(new ActiveSegment(segment.start, segment.length, pointIndex));
                }

                //swap active segments
                var temp = activeSegments;
                activeSegments = newActiveSegments;
                newActiveSegments = temp;
                newActiveSegments.Clear();
            }





            return points;//.Select(x => x.ToVector4()).ToArray();
        }

#if UNITY_EDITOR


        public static string GetMainKernelBody() => Include("PointDetector.MainKernelBody.cginc");

        public static new string GetBufferElementTypeDeclaration() => new Scopes.C.Scopeꓼ($"struct {GetBufferElementTypeName()}") {
            "uint start_length;",
            "float s;",
            "float sx;",
            "float sxx;"
        }.ToString();

        public static new string GetBufferElementTypeName() => "segment";

#endif

        [System.Serializable]
        public struct Segment {
            public ushort length;
            public ushort start;
            
            public float s;
            public float sx;
            public float sxx;
            public Segment(ushort start, ushort length, float s, float sx, float sxx) {
                this.start = start;
                this.length = length;
                this.s = s;
                this.sx = sx;
                this.sxx = sxx;
            }
        }


        protected override int GetStride() => Marshal.SizeOf(typeof(Segment));

    }
}
