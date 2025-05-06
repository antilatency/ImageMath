

#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace ImageMath {
    [FilePath]
    public abstract partial record MaterialOperation : Operation{
        protected static Dictionary<Type, Material> Materials = new();
        private Material? _material;
        protected Material Material{
            get {
                if (_material == null) {
                    _material = FindMaterial();
                }
                return _material;
            }
        }
        
        private Material FindMaterial() {
            if (Materials == null)
                Materials = new();

            Materials.TryGetValue(GetType(), out var material);

            if (!material) {
                var shader = Shader.Find(GetShaderName());
                if (shader == null) {
                    throw new Exception($"Shader not found: {GetShaderName()}");
                }
                material = new Material(shader);
                Materials[GetType()] = material;
            }            
            
            return material;
        }

        public ChannelMask ChannelMask = ChannelMask.All;
        protected void ApplyChannelMask() {
            Shader.SetGlobalInt("ImageMath_ChannelMask", (int)ChannelMask);
        }


        public string GetShaderName() => GetShaderName(GetType());
        #if UNITY_EDITOR
        public static string GetShaderName(Type type) => type.FullName.Replace('.','/');
        public static string GetShaderName(ClassDescription classDescription) => GetShaderName(classDescription.Type);


        public static string GetTemplate(ClassDescription classDescription) => LoadCode("MaterialOperation.Template.cginc");
        public static string GetVertexShader(ClassDescription classDescription) => LoadCode("MaterialOperation.VertexShader.cginc");
        public static string GetFragmentShader(ClassDescription classDescription) => LoadCode("MaterialOperation.FragmentShader.cginc");

        #endif

        protected void ApplyRenderTargetSize(RenderTexture renderTexture) {
            Shader.SetGlobalVector("ImageMath_RenderTargetSize", new Vector4(renderTexture.width, renderTexture.height, 1.0f / renderTexture.width, 1.0f / renderTexture.height));
        }

        protected virtual void RenderTo(RenderTexture renderTexture, int pass, int mipLevel = 0) {
            var previousRT = RenderTexture.active;
            ApplyChannelMask();
            ApplyRenderTargetSize(renderTexture);
            ApplyShaderParameters();

            if (mipLevel ==0){
                Graphics.Blit(null, renderTexture, Material, pass);
            } else {
                RenderTargetIdentifier renderTargetIdentifier = new RenderTargetIdentifier(renderTexture,mipLevel);
                using var commandBuffer = new CommandBuffer();
                commandBuffer.Blit(null, renderTargetIdentifier, Material, pass);
                Graphics.ExecuteCommandBuffer(commandBuffer);
            }
            RenderTexture.active = previousRT;
        }

        public void AssignTo(RenderTexture renderTexture, int mipLevel = 0) {
            RenderTo(renderTexture, 0, mipLevel);
        }

        public void AddTo(RenderTexture renderTexture, int mipLevel = 0) {
            RenderTo(renderTexture, 1, mipLevel);
        }

        public void MultiplyTo(RenderTexture renderTexture, int mipLevel = 0) {
            RenderTo(renderTexture, 2, mipLevel);
        }

        public void AlphaBlendTo(RenderTexture renderTexture, int mipLevel = 0) {
            RenderTo(renderTexture, 3, mipLevel);
        }

        public void MaxTo(RenderTexture renderTexture, int mipLevel = 0) {
            RenderTo(renderTexture, 4, mipLevel);
        }

        public void MinTo(RenderTexture renderTexture, int mipLevel = 0) {
            RenderTo(renderTexture, 5, mipLevel);
        }


    }
}
