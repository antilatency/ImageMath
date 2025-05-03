using System.Reflection;

using UnityEngine;

#nullable enable

namespace ImageMath{
    public class MatrixParameter : Parameter {
        public override string GetPrefix() => "M";
        public override string GetShaderParameterAssignmentCode() {
            return $"Shader.SetGlobalMatrix(\"{GetShaderVariableName()}\", {field.Name});";
        }
        public override string GetHLSLDeclaration() {
            return $"float4x4 {GetShaderVariableName()};\n{GetDefine()}";
        }
        
        private MatrixParameter(FieldInfo field) : base(field) {}

        public new static MatrixParameter? Create(FieldInfo field) {
            if (field.FieldType == typeof(Matrix4x4)){
                return new MatrixParameter(field);
            }
            return null;
        } 
    }
}