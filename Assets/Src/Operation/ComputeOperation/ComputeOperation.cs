#if UNITY_EDITOR
#endif

#nullable enable
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Cache;
using UnityEngine;

namespace ImageMath {

    [FilePath]
    public abstract partial record ComputeOperation : Operation
    {

        public static new string GetShaderFileName(ClassDescription classDescription) => classDescription.Type.FullName + ".compute";
        public string GetShaderName() => GetType().FullName;

        protected static Dictionary<System.Type, ComputeShader> ComputeShaders = new();

        private ComputeShader FindComputeShader()
        {
            if (ComputeShaders == null)
                ComputeShaders = new();

            ComputeShaders.TryGetValue(GetType(), out var computeShader);

            if (!computeShader)
            {
                var shader = Resources.Load<ComputeShader>(GetShaderName());
                if (shader == null)
                {
                    throw new System.Exception($"Compute shader not found: {GetShaderName()}");
                }
                computeShader = shader;
                ComputeShaders[GetType()] = computeShader;
            }

            return computeShader;
        }


        private ComputeShader? _computeShader = null;
        public ComputeShader ComputeShader
        {
            get
            {
                if (_computeShader == null)
                {
                    _computeShader = FindComputeShader();
                }
                return _computeShader;
            }
        }

        public abstract ComputeBufferParameters GetComputeBufferParameters();

        public ComputeBuffer CreateComputeBuffer()
        {
            return Static.CreateComputeBuffer(GetComputeBufferParameters());
        }

        public CacheItem<ComputeBuffer> GetTempComputeBuffer()
        {
            var parameters = GetComputeBufferParameters();
            return Static.GetTempComputeBuffer(parameters);
        }

#if UNITY_EDITOR
        public static string GetTemplate() => LoadCode("ComputeOperation.Template.cginc");
        public static string GetBufferElementTypeDeclaration() => "";
        public static string GetBufferElementTypeName() => "float4";
#endif

        protected virtual int GetStride() => Marshal.SizeOf(typeof(float)) * 4;
        protected abstract Vector2Int GetDispatchSize();
        


        protected virtual T[] Execute<T>(){
            using (var buffer = GetTempComputeBuffer())
            {
                return Execute<T>(buffer.Value);                
            }
        }

        protected virtual T[] Execute<T>(ComputeBuffer result){
            var computeShader = ComputeShader;
            ApplyShaderParameters();
            computeShader.SetBuffer(0, "Result", result);
            var dispatchSize = GetDispatchSize();
            computeShader.Dispatch(0, dispatchSize.x, dispatchSize.y, 1);
            var resultArray = new T[result.count];
            result.GetData(resultArray);
            return resultArray;
        }
    }
}
