using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable

namespace ImageMath{
    public class TextureParameter : Parameter {
        public override string GetPrefix() => "T";
        public override string GetShaderParameterAssignmentCode() {
            return $"Shader.SetGlobalTexture(\"{GetShaderVariableName()}\", {_propertyInfo.Name});";
        }

        string GetHLSLTextureType() {
            var type = _propertyInfo.PropertyType;
            if (type == typeof(Texture3D)) {
                return "Texture3D<float4>";
            } else {
                return "Texture2D<float4>";
            }
        }
                
        public override string GetHLSLDeclaration() {
            return
                $"{GetHLSLTextureType()} {GetShaderVariableName()};\n{GetDefine()}"
                + $"\nSamplerState sampler{GetShaderVariableName()};\n#define sampler{_propertyInfo.Name} sampler{GetShaderVariableName()}";
        }
        
        private TextureParameter(PropertyInfo propertyInfo) : base(propertyInfo) {}

        public new static TextureParameter? Create(PropertyInfo propertyInfo) {
            var type = propertyInfo.PropertyType;
            if (type.IsSubclassOf(typeof(Texture)) || type == typeof(Texture)) {
                return new TextureParameter(propertyInfo);
            }
            return null;
        } 
    }
}