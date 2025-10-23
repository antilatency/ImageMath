using UnityEngine;
#nullable enable
namespace ImageMath {
    public class FlatLUT3DRenderable : FlatLUT3DBase {
        public RenderTexture Texture { get; private set; } = null!; // Initialized in CreateTexture
        public FlatLUT3DRenderable(int size) : base(size) {
        }

        protected override void CreateTexture(int size) {
            var dimensions = CalculateDimensions(size);
            Texture = Static.CreateRenderTexture(dimensions.x, dimensions.y);
        }
        public override void SetData(Vector4[] cells, bool _ = true) {
            Texture.SetPixelData(cells, 0);
        }
        public override Vector4[] GetData() {
            var data = Texture.GetPixelData<Vector4>(0);
            return data;
        }
        public override void DestroyTexture() {
            if (Texture) {
                Object.DestroyImmediate(Texture);
            }
        }
    }

}
