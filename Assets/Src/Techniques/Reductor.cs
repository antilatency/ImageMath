using System;
using System.Collections.Generic;
using Cache;
using UnityEngine;

namespace ImageMath {

    public static partial class Static {

        public static Vector4 AverageWeightedByAlpha(this Texture texture, int downScalePerIteration = 4) {
            return Reductor<AverageWeightedByAlphaOperation>.Calculate(texture, downScalePerIteration);
        }
        public static Vector3 AverageWeightedByAlpha_Divided(this Texture texture, int downScalePerIteration = 4) {
            var v = Reductor<AverageWeightedByAlphaOperation>.Calculate(texture, downScalePerIteration);
            if (v.w > 0) {
                return new Vector3(v.x / v.w, v.y / v.w, v.z / v.w);
            } else {
                return new Vector3(0, 0, 0);
            }
        }

        public static Vector4 Maximum(this Texture texture, int downScalePerIteration = 4) {
            return Reductor<MaxOperation>.Calculate(texture, downScalePerIteration);
        }
        public static Vector4 Minimum(this Texture texture, int downScalePerIteration = 4) {
            return Reductor<MinOperation>.Calculate(texture, downScalePerIteration);
        }
    }

    public record Reductor<T> where T : ReductionOperation, new() {

        static int DivideAndRoundUp(int dividend, int divisor) {
            return (dividend + divisor - 1) / divisor;
        }

        public static Vector4 Calculate(Texture texture, int downScalePerIteration = 4) {
            downScalePerIteration = Math.Max(2, downScalePerIteration);

            var width = texture.width;
            var height = texture.height;



            List<CacheItem<RenderTexture>> mips = new();

            while (true) {
                if (width == 1 && height == 1)
                    break;

                width = DivideAndRoundUp(width , downScalePerIteration);
                height = DivideAndRoundUp(height , downScalePerIteration);

                mips.Add(Static.GetTempRenderTexture(width, height));
            }

            var operation = new T();
            Texture source = texture;

            for (int i = 0; i < mips.Count; i++) {
                var dest = mips[i].Value;
                operation.Texture = source;
                operation.AssignTo(dest);
                source = dest;
            }

            var result = source.GetPixelData<Vector4>();
            for (int i = 0; i < mips.Count; i++) {
                mips[i].Dispose();
            }

            return result[0];
        }
    }

}
