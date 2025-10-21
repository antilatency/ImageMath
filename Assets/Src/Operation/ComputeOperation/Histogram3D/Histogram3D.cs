#nullable enable
using System.Linq;
using System.Runtime.InteropServices;

using UnityEngine;
using UnityEngine.Rendering;

namespace ImageMath {
    [FilePath]
    public partial record Histogram3D : ComputeOperation {
        public Texture Texture { get; set; }
        public Vector3Int Size { get; set; }

        public Histogram3D(Texture texture, Vector3Int? size = null) {
            Texture = texture;
            Size = size ?? new Vector3Int(64, 64, 64);
        }

        public override ComputeBufferParameters GetComputeBufferParameters() {
            return new ComputeBufferParameters(
                Size.x * Size.y * Size.z,
                Marshal.SizeOf(typeof(int)),
                ComputeBufferType.Default,
                ComputeBufferMode.Immutable
            );
        }

        protected override Vector3Int GetDispatchSize() {
            if (Texture == null) {
                throw new System.Exception("Texture is null");
            }
            var width = Texture.width;
            var height = Texture.height;

            var computeShader = ComputeShader;
            computeShader.GetKernelThreadGroupSizes(0, out var threadGroupSizeX, out var threadGroupSizeY, out var threadGroupSizeZ);

            int groupsX = Mathf.CeilToInt(width / (float)threadGroupSizeX);
            int groupsY = Mathf.CeilToInt(height / (float)threadGroupSizeY);
            return new Vector3Int(groupsX, groupsY, 1);
        }
#if UNITY_EDITOR
        public static string GetMainKernelBody() => Embed($"{nameof(Histogram3D)}.MainKernelBody.cginc");
        public static new string GetBufferElementTypeName() => "int";
#endif

        public int[] Execute(ComputeBuffer? computeBuffer = null) {
            Cache.CacheItem<ComputeBuffer>? computeBufferCacheItem = null;

            using var disposer = new Disposable(() => {
                if (computeBufferCacheItem != null) {
                    computeBufferCacheItem.Dispose();
                }
            });

            if (computeBuffer == null) {
                computeBufferCacheItem = GetTempComputeBuffer();
                computeBuffer = computeBufferCacheItem.Value;
            }
            computeBuffer.SetData(new int[Size.x * Size.y * Size.z]);
            return ExecuteUsingExternalBuffer<int>(computeBuffer);
        }

        public static float[] Normalize(int[] values) {
            if (values == null || values.Length == 0) {
                return new float[0];
            }
            var max = values.Max();
            if (max == 0) {
                return new float[values.Length];
            }
            var normalized = new float[values.Length];
            for (int i = 0; i < values.Length; i++) {
                normalized[i] = values[i] / (float)max;
            }
            return normalized;
        }

    }
}
