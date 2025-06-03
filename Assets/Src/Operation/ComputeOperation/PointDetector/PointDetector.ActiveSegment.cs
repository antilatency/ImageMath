#nullable enable
namespace ImageMath {
    public partial record PointDetector {
        public class ActiveSegment {
            public int Start;
            public int Length;
            public int PointIndex;
            public ActiveSegment(int start, int length, int pointIndex) {
                Start = start;
                Length = length;
                PointIndex = pointIndex;
            }


            public bool IntersectsWith(PointDetector.Segment segment) {
                if (Start + Length <= segment.start)
                    return false;
                if (Start >= segment.start + segment.length)
                    return false;
                return true;
            }
        }
    }
}
