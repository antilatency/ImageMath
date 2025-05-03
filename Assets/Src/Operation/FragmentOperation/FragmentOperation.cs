using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


#nullable enable
namespace ImageMath {
    public abstract partial record FragmentOperation: Operation {
        public ChannelMask ChannelMask = ChannelMask.All;
        protected void ApplyChannelMask() {
            Shader.SetGlobalInt("ImageMath_ChannelMask", (int)ChannelMask);
        }

        public Vector2 Position = Vector2.zero;
        public Vector2 Size = Vector2.one;


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


 


        protected void RenderTo(RenderTexture renderTexture, int pass, int mipLevel = 0) {
            var previousRT = RenderTexture.active;
            ApplyChannelMask();
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

        #if UNITY_EDITOR
        public static string GetTemplate(ClassDescription classDescription) => LoadCode("FragmentOperation.Template.cginc");
        public static string GetVertexShader(ClassDescription classDescription) => LoadCode("FragmentOperation.VertexShader.cginc");
        public static string GetFragmentShader(ClassDescription classDescription) => LoadCode("FragmentOperation.FragmentShader.cginc");
        #endif



    }
}
