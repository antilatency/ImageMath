#if UNITY_EDITOR
#endif

#nullable enable
using System.Collections.Generic;
using Cache;
using UnityEngine;

namespace ImageMath {

    [FilePath]
    public abstract partial record ComputeOperation: Operation {

        public static new string GetShaderFileName(ClassDescription classDescription) => classDescription.Type.FullName+".compute"; 
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

        public CacheItem<ComputeBuffer> GetTempComputeBuffer(){
            var parameters = GetComputeBufferParameters();
            return Static.GetTempComputeBuffer(parameters);
        }

        #if UNITY_EDITOR
        public static string GetTemplate() => LoadCode("ComputeOperation.Template.cginc");
        #endif

        /*public void Execute(ComputeBuffer result){
            Shader.SetGlobalBuffer
        }*/
    }
}
