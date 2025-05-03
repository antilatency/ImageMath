using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;

#nullable enable

namespace ImageMath{
    public class VectorParameter : Parameter {
        static readonly Dictionary<Type, int> vectorTypes = new Dictionary<Type, int>(){
            {typeof(Vector2), 2},
            {typeof(Vector3), 3},
            {typeof(Vector4), 4}
        };

        public override string GetPrefix() => "V";
        public override string GetShaderParameterAssignmentCode() {
            return $"Shader.SetGlobalVector(\"{GetShaderVariableName()}\", {field.Name});";
        }
        public override string GetHLSLDeclaration() {
            var size = vectorTypes[field.FieldType];
            return $"float{size} {GetShaderVariableName()};\n{GetDefine()}";
        }
        
        private VectorParameter(FieldInfo field) : base(field) {}

        public new static VectorParameter? Create(FieldInfo field) {
            if (vectorTypes.ContainsKey(field.FieldType)){
                return new VectorParameter(field);
            }
            return null;
        } 
    }
}