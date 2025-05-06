using System;
using System.Collections.Generic;
using Cache;
using UnityEngine;

namespace ImageMath {

    public static partial class Static {

        public static Vector4 AverageWeightedByAlpha(this Texture texture, int downSclalePerIteration = 4) {
            return Reductor<AverageWeightedByAlphaOperation>.Calculate(texture, downSclalePerIteration);
        }
        public static Vector4 Maximum(this Texture texture, int downSclalePerIteration = 4) {
            return Reductor<MaxOperation>.Calculate(texture, downSclalePerIteration);
        }
        public static Vector4 Minimum(this Texture texture, int downSclalePerIteration = 4) {
            return Reductor<MinOperation>.Calculate(texture, downSclalePerIteration);
        }
    }

    public record Reductor<T> where T : ReductionOperation, new() {

        static int DivideAndRoundUp(int dividend, int divisor) {
            return (dividend + divisor - 1) / divisor;
        }

        public static Vector4 Calculate(Texture texture, int downSclalePerIteration = 4) {
            downSclalePerIteration = Math.Max(2, downSclalePerIteration);

            var width = texture.width;
            var height = texture.height;

            

            List<CacheItem<RenderTexture>> mips = new();

            while (true) {
                if (width == 1 && height == 1)
                    break;

                width = DivideAndRoundUp(width , downSclalePerIteration);
                height = DivideAndRoundUp(height , downSclalePerIteration);

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

            var result = source.GetRawTextureData();
            for (int i = 0; i < mips.Count; i++) {
                mips[i].Dispose();
            }
            
            return result[0];
        }
    }

}
