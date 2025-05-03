using UnityEngine;
using UnityEngine.Rendering;

namespace ImageMath {

    public static class FragmentOperationExtension{
        public static T FlipY<T>(this T _this) where T : FragmentOperation {
            _this.Position.y += _this.Size.y;
            _this.Size.y = -_this.Size.y;
            return _this;
        }

        public static T FlipX<T>(this T _this) where T : FragmentOperation {
            _this.Position.x += _this.Size.x;
            _this.Size.x = -_this.Size.x;
            return _this;
        }

        public static T Tile<T>(this T _this, int index, int numTilesX, int numTilesY) where T : FragmentOperation {
            Vector2 tileSize = new Vector2(1f / numTilesX, 1f / numTilesY);
            _this.Position  = new Vector2Int(index % numTilesX, index / numTilesX) * tileSize;
            _this.Size = tileSize;
            return _this;
        }
        public static T Tile<T>(this T _this, int index, Vector2Int numTiles) where T : FragmentOperation {
            return _this.Tile(index, numTiles.x, numTiles.y);
        }

        public static T Crop<T>(this T _this, Vector2 size, Vector2 position) where T : FragmentOperation {
            var invSize = new Vector2(1/size.x, 1/size.y);
            return _this with { 
                Size = invSize,
                Position = -(invSize - Vector2.one) * position
            };
        }
    }


    

}
