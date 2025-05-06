using UnityEngine;

namespace ImageMath {
    public record ComputeBufferParameters{
        public int Count;
        public int Stride;
        public ComputeBufferType Type;
        public ComputeBufferMode Mode;
        public ComputeBufferParameters(int count, int stride
        , ComputeBufferType type = ComputeBufferType.Default
        , ComputeBufferMode mode = ComputeBufferMode.Immutable) {
            Count = count;
            Stride = stride;
            Type = type;
            Mode = mode;
        }
    }

}
