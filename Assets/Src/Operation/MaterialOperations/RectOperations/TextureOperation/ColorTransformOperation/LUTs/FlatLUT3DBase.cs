using System;
using UnityEngine;
#nullable enable
namespace ImageMath {
    public abstract class FlatLUT3DBase<T> : LUT3DBase<T> where T : Texture {

        public FlatLUT3DBase(int size, string? title = null, Vector3? domainMin = null, Vector3? domainMax = null): base(size, title, domainMin, domainMax) {
        }

        public Vector2Int CalculateDimensions(int size) {
            int numPixels = size * size * size;
            int width;
            int height;

            if (size* size <= 4096){
                width = size;
                height = size*size;
            } else {

                width = (int)Math.Ceiling(Math.Sqrt(numPixels));
                height = (int)Math.Ceiling((float)numPixels / width);
            }
            return new Vector2Int(width, height);
        }

        public LUT3D ToLUT3D() {
            if (Texture == null) {
                throw new Exception("LUT3DFlatBase: Texture is null, cannot convert to LUT3D.");
            }
            var size = Size;
            var data = Texture.GetRawTextureData();
            var result = new LUT3D(size, Title, DomainMin, DomainMax);
            result.SetData(data);
            return result;
        }


    }

}
