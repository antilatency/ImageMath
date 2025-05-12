using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
#nullable enable
namespace ImageMath{
    public class StructParameter : Parameter {

        public static readonly Dictionary<Type, (string prefix, string hlslType, string hlslTypeSize, string setGlobalMethodName)> SupportedTypes = new (){
            {typeof(float), ("F", "float", "", "SetGlobalFloat")},
            {typeof(int), ("I", "int", "", "SetGlobalInt")},
            {typeof(Matrix4x4), ("M", "float", "4x4", "SetGlobalMatrix")},
            {typeof(Vector2), ("V", "float", "2", "SetGlobalVector")},
            {typeof(Vector3), ("V", "float", "3", "SetGlobalVector")},
            {typeof(Vector4), ("V", "float", "4", "SetGlobalVector")},
        };


        public override string GetPrefix() => SupportedTypes[_propertyInfo.PropertyType].prefix;
        

        public override string GetShaderParameterAssignmentCode() {
            return $"Shader.{SupportedTypes[_propertyInfo.PropertyType].setGlobalMethodName}(\"{GetShaderVariableName()}\", {_propertyInfo.Name});";
        }

        public override string GetHLSLDeclaration() {
            var info = SupportedTypes[_propertyInfo.PropertyType];
            return $"{info.hlslType}{info.hlslTypeSize} {GetShaderVariableName()};\n{GetDefine()}";
        }

        private StructParameter(PropertyInfo propertyInfo) : base(propertyInfo) {}

        public static new StructParameter? Create(PropertyInfo propertyInfo) {
            if (SupportedTypes.ContainsKey(propertyInfo.PropertyType)){
                return new StructParameter(propertyInfo);
            }
            return null;
        }
    }
}