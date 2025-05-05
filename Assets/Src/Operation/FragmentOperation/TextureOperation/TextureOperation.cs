using ImageMath;
using UnityEngine;
namespace ImageMath{
    [FilePath]
    public abstract partial record TextureOperation: FragmentOperation {
        public Texture Texture;

        public TextureOperation(Texture texture) {
            Texture = texture;
        }
        public TextureOperation() {
            Texture = null;
        }
    }
}
