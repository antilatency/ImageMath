using System;
using System.Collections.Generic;
using System.Reflection;

namespace ImageMath{
    public class ScalarParameter : Parameter {

        static readonly Dictionary<Type, (string prefix, string hlslType, string setGlobalMethodName)> types = new (){
            {typeof(float), ("F", "float", "SetGlobalFloat")},
            {typeof(int), ("I", "int", "SetGlobalInt")},
        };


        public override string GetPrefix() => types[field.FieldType].prefix;
        

        public override string GetShaderParameterAssignmentCode() {
            return $"Shader.{types[field.FieldType].setGlobalMethodName}(\"{GetShaderVariableName()}\", {field.Name});";
        }

        public override string GetHLSLDeclaration() {
            return $"{types[field.FieldType].hlslType} {GetShaderVariableName()};\n{GetDefine()}";
        }

        private ScalarParameter(FieldInfo field) : base(field) {}

        public static ScalarParameter? Create(FieldInfo field) {
            if (types.ContainsKey(field.FieldType)){
                return new ScalarParameter(field);
            }
            return null;
        }
    }
}