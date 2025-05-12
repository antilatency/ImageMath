using UnityEngine;
#nullable enable
namespace ImageMath {
    public class FlatLUT3DRenderable : FlatLUT3DBase<RenderTexture> {
        public FlatLUT3DRenderable(int size, string? title = null, Vector3? domainMin = null, Vector3? domainMax = null): base(size, title, domainMin, domainMax) {
        }

        protected override RenderTexture CreateTexture(int size) {
            var dimensions = CalculateDimensions(size);
            return Static.CreateRenderTexture(dimensions.x, dimensions.y);
        }        
    }

}
