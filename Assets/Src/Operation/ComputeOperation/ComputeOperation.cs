#if UNITY_EDITOR
#endif

#nullable enable
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Cache;
using UnityEngine;

namespace ImageMath {

    [FilePath]
    public abstract partial record ComputeOperation : Operation {

        public static new string GetShaderFileName(ClassDescription classDescription) => classDescription.Type.FullName + ".compute";
        public string GetShaderName() => GetType().FullName;

        protected static Dictionary<System.Type, ComputeShader> ComputeShaders = new();

        private ComputeShader FindComputeShader() {
            if (ComputeShaders == null)
                ComputeShaders = new();

            ComputeShaders.TryGetValue(GetType(), out var computeShader);

            if (!computeShader) {
                var shader = Resources.Load<ComputeShader>(GetShaderName());
                if (shader == null) {
                    throw new System.Exception($"Compute shader not found: {GetShaderName()}");
                }
                computeShader = shader;
                ComputeShaders[GetType()] = computeShader;
            }

            return computeShader;
        }


        private ComputeShader? _computeShader = null;
        public ComputeShader ComputeShader {
            get {
                if (_computeShader == null) {
                    _computeShader = FindComputeShader();
                }
                return _computeShader;
            }
        }

        public abstract ComputeBufferParameters GetComputeBufferParameters();

        public ComputeBuffer CreateComputeBuffer() {
            return Static.CreateComputeBuffer(GetComputeBufferParameters());
        }

        public CacheItem<ComputeBuffer> GetTempComputeBuffer() {
            var parameters = GetComputeBufferParameters();
            return Static.GetTempComputeBuffer(parameters);
        }

#if UNITY_EDITOR
        public static string GetTemplate() => LoadCode("ComputeOperation.Template.cginc");
        public static string GetBufferElementTypeDeclaration() => "";
        public static string GetBufferElementTypeName() => "float4";
#endif

        protected virtual int GetStride() => Marshal.SizeOf(typeof(float)) * 4;
        protected abstract Vector3Int GetDispatchSize();

        protected virtual T[] ExecuteInternal<T>(ComputeBuffer? result = null) {
            if (result == null) {
                return ExecuteInternalTempBuffer<T>();
            }
            return ExecuteUsingExternalBuffer<T>(result);
        }

        protected virtual T[] ExecuteInternalTempBuffer<T>() {
            using (var buffer = GetTempComputeBuffer()) {
                return ExecuteInternal<T>(buffer.Value);
            }
        }


        protected virtual T[] ExecuteUsingExternalBuffer<T>(ComputeBuffer result) {
            var computeShader = ComputeShader;
            ApplyShaderParameters();
            ApplyCustomShaderParameters();
            computeShader.SetBuffer(0, "Result", result);
            var dispatchSize = GetDispatchSize();
            computeShader.Dispatch(0, dispatchSize.x, dispatchSize.y, dispatchSize.z);
            var resultArray = new T[result.count];
            result.GetData(resultArray);
            return resultArray;
        }


        public override void SetFloat(string name, float value) => ComputeShader.SetFloat(name, value);
        public override void SetFloatArray(string name, float[] values) => throw new System.NotImplementedException("Compute shaders do not support float arrays directly. Use a ComputeBuffer instead.");

        public override void SetInt(string name, int value) => ComputeShader.SetInt(name, value);

        public override void SetVector(string name, Vector4 value) => ComputeShader.SetVector(name, value);
        public override void SetVectorArray(string name, Vector4[] values) => ComputeShader.SetVectorArray(name, values);

        public override void SetMatrix(string name, Matrix4x4 value) => ComputeShader.SetMatrix(name, value);
        public override void SetMatrixArray(string name, Matrix4x4[] values) => ComputeShader.SetMatrixArray(name, values);

        public override void SetTexture(string name, Texture value) => ComputeShader.SetTexture(0, name, value);
        
        public override void EnableKeyword(string name) => ComputeShader.EnableKeyword(name);
        public override void DisableKeyword(string name) => ComputeShader.DisableKeyword(name);
    }
}
