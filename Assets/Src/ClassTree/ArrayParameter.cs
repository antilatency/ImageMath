using System;
using System.Reflection;

#nullable enable

namespace ImageMath{
    public class ArrayParameter : Parameter {
        private bool _isDynamicArray;
        private int _size;
        private Type _elementType;

        public string GetElementTypePrefix() {
            var info = StructParameter.SupportedTypes[_elementType];
            return info.prefix;
        }

        public override string GetPrefix() => $"A{_size}{GetElementTypePrefix()}";
        public override string GetShaderParameterAssignmentCode() {
            var info = StructParameter.SupportedTypes[_elementType];
            var result = $"Shader.{info.setGlobalMethodName}Array(\"{GetShaderVariableName()}\", {field.Name});";
            if (_isDynamicArray) {
                result = $"{result}\nShader.SetGlobalInt(\"{GetShaderVariableName()}_Size\", {field.Name}.Length);";
            } else {
                result = $"if ({field.Name}.Length != {_size}) {{\n" +
                         $"    throw new Exception(\"Array size mismatch for {field.Name}. Expected {_size}, got \" + {field.Name}.Length);\n" +
                         "} else {\n" +
                         $"{result}\n" +
                         "}";
            }
            return result; 
        }

        public override string GetHLSLDeclaration() {
            var result = $"float4 {GetShaderVariableName()}[{_size}];\n{GetDefine()}";
            if (_isDynamicArray) {
                result = $"{result}\nint {GetShaderVariableName()}_Size;\n#define {field.Name}_Size {GetShaderVariableName()}_Size";
            } else {
                result = $"{result}\n#define {field.Name}_Size {_size}";
            }
            return result;
        }
        
        private ArrayParameter(FieldInfo field, bool isDynamicArray, int size) : base(field) {
            _elementType = field.FieldType.GetElementType()!;
            _isDynamicArray = isDynamicArray;
            _size = size;
        }



        public new static ArrayParameter? Create(FieldInfo field) {
            if (field.FieldType.IsArray){
                var elementType = field.FieldType.GetElementType()!;

                if (!StructParameter.SupportedTypes.ContainsKey(elementType)) return null;

                var arrayAttribute = field.GetCustomAttribute<ArrayAttribute>();
                var isDynamicArray = arrayAttribute is DynamicArrayAttribute;
                var size = arrayAttribute.Size;

                return new ArrayParameter(field, isDynamicArray, size);
            }
            return null;
        } 
    }
}