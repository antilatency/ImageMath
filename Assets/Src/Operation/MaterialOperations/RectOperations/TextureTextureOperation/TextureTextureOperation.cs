using UnityEngine;
namespace ImageMath {
    [FilePath]
    public abstract partial record TextureTextureOperation : RectOperation {
        public Texture TextureA { get; set; }
        public Texture TextureB { get; set; }

        public TextureTextureOperation(Texture textureA, Texture textureB) {
            TextureA = textureA;
            TextureB = textureB;
        }
        public TextureTextureOperation() {
            TextureA = null;
            TextureB = null;
        }
    }
}
