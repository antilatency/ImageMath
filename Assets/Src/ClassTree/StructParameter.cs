using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
#nullable enable
namespace ImageMath{

    public static class ShaderSetGlobalUtils {
        public static void SetGlobalVectorInt(string name, Vector2Int value) {
            Shader.SetGlobalVector(name, new Vector4(value.x, value.y, 0, 0));
        }
        public static void SetGlobalVectorInt(string name, Vector3Int value) {
            Shader.SetGlobalVector(name, new Vector4(value.x, value.y, value.z, 0));
        }
    }

    public class StructParameter : Parameter {

        public static readonly Dictionary<Type, (string prefix, string hlslType, string hlslTypeSize, string setMethodName)> SupportedTypes = new(){
            {typeof(float), ("F", "float", "", "SetFloat")},
            {typeof(int), ("I", "int", "", "SetInt")},
            {typeof(Matrix4x4), ("M", "float", "4x4", "SetMatrix")},
            {typeof(Vector2), ("V", "float", "2", "SetVector")},
            {typeof(Vector3), ("V", "float", "3", "SetVector")},
            {typeof(Vector4), ("V", "float", "4", "SetVector")},

            {typeof(Vector2Int), ("V", "float", "2", "ShaderSetGlobalUtils.SetGlobalVectorInt")},
            {typeof(Vector3Int), ("V", "float", "3", "ShaderSetGlobalUtils.SetGlobalVectorInt")},
        };



        //public override string GetPrefix() => SupportedTypes[_propertyInfo.PropertyType].prefix;


        public override string GetShaderParameterAssignmentCode() {
            return $"{SupportedTypes[_propertyInfo.PropertyType].setMethodName}(\"{GetShaderVariableName()}\", {_propertyInfo.Name});";
        }

        public override string GetHLSLDeclaration() {
            var info = SupportedTypes[_propertyInfo.PropertyType];
            return $"{info.hlslType}{info.hlslTypeSize} {GetShaderVariableName()};";
        }

        private StructParameter(PropertyInfo propertyInfo) : base(propertyInfo) { }

        public static new StructParameter? Create(PropertyInfo propertyInfo) {
            if (SupportedTypes.ContainsKey(propertyInfo.PropertyType)) {
                return new StructParameter(propertyInfo);
            }
            return null;
        }
    }
}