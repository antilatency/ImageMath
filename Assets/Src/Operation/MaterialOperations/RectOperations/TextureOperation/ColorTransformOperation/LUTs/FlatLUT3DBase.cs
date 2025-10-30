using System;


using UnityEngine;
#nullable enable
namespace ImageMath {
    [Serializable]
    public abstract class FlatLUT3DBase : LUT3DBase {

        public FlatLUT3DBase(int size) : base(size) {
        }

        public static bool operator true(FlatLUT3DBase? lut) {
            return lut != null && lut.Texture;
        }

        public static bool operator false(FlatLUT3DBase? lut) {
            return lut == null || !lut.Texture;
        }

        public Vector2Int CalculateDimensions(int size) {
            int numPixels = size * size * size;
            int width;
            int height;

            if (size * size <= 4096) {
                width = size;
                height = size * size;
            }
            else {

                width = (int)Math.Ceiling(Math.Sqrt(numPixels));
                height = (int)Math.Ceiling((float)numPixels / width);
            }
            return new Vector2Int(width, height);
        }

        public LUT3D ToLUT3D() {
            var size = Size;
            var data = GetData();
            var result = new LUT3D(size) {
                Title = Title,
                DomainMin = DomainMin,
                DomainMax = DomainMax
            };
            result.SetData(data);
            return result;
        }


    }

}
