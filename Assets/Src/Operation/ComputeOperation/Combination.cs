using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ImageMath
{
    public record Combination : IEnumerable
    {
        public ChannelMask ChannelMask = ChannelMask.All;

        List<(Texture, Vector4)> _pairs = new();
        public Combination() { }

        public Combination(IEnumerable<(Texture, Vector4)> elements)
        {
            _pairs = new(elements);
        }

        public void Add(Texture texture, Vector4 multiplier)
        {
            _pairs.Add((texture, multiplier));
        }
        public void Add(Texture texture, float multiplier)
        {
            Add(texture, new Vector4(multiplier, multiplier, multiplier, multiplier));
        }

        public void RenderTo(RenderTexture renderTexture)
        {
            if (_pairs.Count == 0)
            {
                new TextureMultipliedByVector
                {
                    ChannelMask = ChannelMask,
                    Texture = Texture2D.blackTexture,
                    Multiplier = Vector4.zero,
                }.AssignTo(renderTexture);
            }
            else
            {
                new TextureMultipliedByVector
                {
                    ChannelMask = ChannelMask,
                    Texture = _pairs[0].Item1,
                    Multiplier = _pairs[0].Item2,
                }.AssignTo(renderTexture);
            }

            for (int i = 1; i < _pairs.Count; i++)
            {
                new TextureMultipliedByVector
                {
                    ChannelMask = ChannelMask,
                    Texture = _pairs[i].Item1,
                    Multiplier = _pairs[i].Item2,
                }.AddTo(renderTexture);
            }
        }

        public IEnumerator GetEnumerator()
        {
            return _pairs.GetEnumerator();
        }
    }
}
