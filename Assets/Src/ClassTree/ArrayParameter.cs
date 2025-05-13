using System;
using System.Reflection;

using Codice.Utils;
using Scopes;
using Scopes.C;

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
            var array = new float[5];
            
            var result = $"Shader.{info.setGlobalMethodName}Array(\"{GetShaderVariableName()}\", ExpandArray({_propertyInfo.Name},{_size}));";
            if (_isDynamicArray) {
                result = $"{result}\nShader.SetGlobalInt(\"{GetShaderVariableName()}_Size\", {_propertyInfo.Name}.Length);";
            } else {
                result = $"if ({_propertyInfo.Name}.Length != {_size}) {{\n" +
                         $"    throw new Exception(\"Array size mismatch for {_propertyInfo.Name}. Expected {_size}, got \" + {_propertyInfo.Name}.Length);\n" +
                         "} else {\n" +
                         $"{result}\n" +
                         "}";
            }
            return result; 
        }

        public override string GetHLSLDeclaration() {
            var result = $"float4 {GetShaderVariableName()}[{_size}];\n{GetDefine()}";
            if (_isDynamicArray) {
                result = $"{result}\nint {GetShaderVariableName()}_Size;\n#define {_propertyInfo.Name}_Size {GetShaderVariableName()}_Size";
            } else {
                result = $"{result}\n#define {_propertyInfo.Name}_Size {_size}";
            }
            return result;
        }
        
        private ArrayParameter(PropertyInfo propertyInfo, bool isDynamicArray, int size) : base(propertyInfo) {
            _elementType = propertyInfo.PropertyType.GetElementType()!;
            _isDynamicArray = isDynamicArray;
            _size = size;
        }



        public new static ArrayParameter? Create(PropertyInfo propertyInfo) {
            var type = propertyInfo.PropertyType;
            if (type.IsArray){
                var elementType = type.GetElementType()!;

                if (!StructParameter.SupportedTypes.ContainsKey(elementType)) return null;

                var arrayAttribute = propertyInfo.GetCustomAttribute<ArrayAttribute>();
                var isDynamicArray = arrayAttribute is DynamicArrayAttribute;
                var size = arrayAttribute.Size;

                return new ArrayParameter(propertyInfo, isDynamicArray, size);
            }
            return null;
        } 
    }
}