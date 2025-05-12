using System;
using UnityEngine;
#nullable enable
namespace ImageMath {
    public class FlatLUT3D : FlatLUT3DBase<Texture2D> {
        public FlatLUT3D(int size, string? title = null, Vector3? domainMin = null, Vector3? domainMax = null): base(size, title, domainMin, domainMax) {
        }

        protected override Texture2D CreateTexture(int size) {
            var dimensions = CalculateDimensions(size);
            return Static.CreateTexture2DFloat4(dimensions.x, dimensions.y);
        }   

        public static FlatLUT3D? CreateFromCubeFileContent(string content, bool apply = true) {
            return ParseLUT3D(content,
                (size, data) => {
                    var lut = new FlatLUT3D(size, null, Vector3.zero, Vector3.one);
                    lut.SetData(data, apply);
                    return lut;
                });
        }

        public FlatLUT3DRenderable CreateCompatibleRenderable() {
            return new FlatLUT3DRenderable(Size, Title, DomainMin, DomainMax);
        }

    }

}
