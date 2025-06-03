#nullable enable
using System.Collections.Generic;

namespace ImageMath {
    public static class PointDetectorActiveSegmentListExtensions {


        public static void RemoveIndex(this IList<PointDetector.ActiveSegment> segments, int index, int replacementIndex) {
            foreach (var segment in segments) {
                if (segment.PointIndex == index) {
                    segment.PointIndex = replacementIndex;
                } else if (segment.PointIndex > index) {
                    segment.PointIndex--;
                }
            }
        }
              
    
    }
}
