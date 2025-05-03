using System.Reflection;

using UnityEngine;

#nullable enable

namespace ImageMath{
    public class TextureParameter : Parameter {
        public override string GetPrefix() => "T";
        public override string GetShaderParameterAssignmentCode() {
            return $"Shader.SetGlobalTexture(\"{GetShaderVariableName()}\", {field.Name});";
        }
        public override string GetHLSLDeclaration() {
            return
                $"Texture2D<float4> {GetShaderVariableName()};\n{GetDefine()}"
                + $"\nSamplerState sampler{GetShaderVariableName()};\n#define sampler{field.Name} sampler{GetShaderVariableName()}";
        }
        
        private TextureParameter(FieldInfo field) : base(field) {}

        public new static TextureParameter? Create(FieldInfo field) {
            if (field.FieldType.IsSubclassOf(typeof(Texture)) || field.FieldType == typeof(Texture)) {
                return new TextureParameter(field);
            }
            return null;
        } 
    }
}