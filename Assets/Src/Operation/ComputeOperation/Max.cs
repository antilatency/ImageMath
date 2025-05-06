#if UNITY_EDITOR
#endif

#nullable enable
using UnityEngine;

namespace ImageMath {
    [FilePath]
    public partial record Max: ReduceOperation {
        public Texture? Texture;

        public Vector2Int GetDispatchSize(){
            if (Texture == null) {
                throw new System.Exception("Texture is null");
            }
            var width = Texture.width;
            var height = Texture.height;
            var computeShader = ComputeShader;
            computeShader.GetKernelThreadGroupSizes(0, out var threadGroupSizeX, out var threadGroupSizeY, out var threadGroupSizeZ);

            int groupsX = Mathf.CeilToInt(width / (float)threadGroupSizeX);
            int groupsY = Mathf.CeilToInt(height / (float)threadGroupSizeY);
            return new Vector2Int(groupsX, groupsY);
        }

        

        public override ComputeBufferParameters GetComputeBufferParameters() {
            var dispatchSize = GetDispatchSize();
            var bufferSize = dispatchSize.x * dispatchSize.y;
            return new ComputeBufferParameters(bufferSize, sizeof(float)*4, ComputeBufferType.Default, ComputeBufferMode.Immutable);
        }

        public Vector4 Execute(){
            using (var buffer = GetTempComputeBuffer()) {
                var computeShader = ComputeShader;
                ApplyShaderParameters();
                computeShader.SetBuffer(0, "Result", buffer.Value);
                var dispatchSize = GetDispatchSize();
                computeShader.Dispatch(0, dispatchSize.x,dispatchSize.y, 1);
                var resultArray = new Vector4[dispatchSize.x * dispatchSize.y];
                buffer.Value.GetData(resultArray);
                var result = new Vector4(float.MinValue, float.MinValue, float.MinValue, float.MinValue);
                for (int i = 0; i < resultArray.Length; i++) {
                    result.x = Mathf.Max(result.x, resultArray[i].x);
                    result.y = Mathf.Max(result.y, resultArray[i].y);
                    result.z = Mathf.Max(result.z, resultArray[i].z);
                    result.w = Mathf.Max(result.w, resultArray[i].w);
                }

                return result;
            }
        }
    }
}
