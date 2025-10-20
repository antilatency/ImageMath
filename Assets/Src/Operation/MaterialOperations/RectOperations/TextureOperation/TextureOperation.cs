using ImageMath;
using UnityEngine;
namespace ImageMath {
    [FilePath]
    public abstract partial record TextureOperation : RectOperation {
        public Texture Texture { get; set; }

        public TextureOperation(Texture texture) {
            Texture = texture;
        }
        public TextureOperation() {
            Texture = null;
        }
    }
}
