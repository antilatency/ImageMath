using System;
using System.Collections.Generic;
using System.Reflection;



#nullable enable

namespace ImageMath{
    public class ArrayParameter : Parameter {
        private string _sizeAccessor;
        private bool _isDynamicArray;
        private int _size;
        private Type _elementType;

        public string GetElementTypePrefix() {
            var info = StructParameter.SupportedTypes[_elementType];
            return info.prefix;
        }

        //public override string GetPrefix() => $"A{_size}{GetElementTypePrefix()}";
        public override string GetShaderParameterAssignmentCode() {
            var info = StructParameter.SupportedTypes[_elementType];
            
            var result = $"{info.setMethodName}Array(\"{GetShaderVariableName()}\", ExpandArray({_propertyInfo.Name},{_size}));";
            if (_isDynamicArray) {
                result = $"{result}\nShader.SetGlobalInt(\"{GetShaderVariableName()}_Size\", {_propertyInfo.Name}.{_sizeAccessor});";
            } else {
                /*result = $"if ({_propertyInfo.Name}.Length != {_size}) {{\n" +
                         $"    throw new Exception(\"Array size mismatch for {_propertyInfo.Name}. Expected {_size}, got \" + {_propertyInfo.Name}.Length);\n" +
                         "} else {\n" +
                         $"{result}\n" +
                         "}";*/
            }
            return result; 
        }

        public override string GetHLSLDeclaration() {
            var hlslElementTypeInfo = StructParameter.SupportedTypes[_elementType];
            var hlslTypeName = hlslElementTypeInfo.hlslType + hlslElementTypeInfo.hlslTypeSize;
            var result = $"{hlslTypeName} {GetShaderVariableName()}[{_size}];";
            if (_isDynamicArray) {
                result = $"{result}\nint {GetShaderVariableName()}_Size;";
            } else {
                result = $"{result}\n#define {_propertyInfo.Name}_Size {_size}";
            }
            return result;
        }

        private ArrayParameter(PropertyInfo propertyInfo, Type elementType, bool isDynamicArray, int size) : base(propertyInfo) {
            _sizeAccessor = propertyInfo.PropertyType.IsArray ? "Length" : "Count";
            _elementType = elementType;
            _isDynamicArray = isDynamicArray;
            _size = size;
        }



        public new static ArrayParameter? Create(PropertyInfo propertyInfo) {
            var type = propertyInfo.PropertyType;
            
            bool typeIsGenericIList = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IList<>);
            if (!type.IsArray && !typeIsGenericIList) return null;

            Type? elementType = type.IsArray ? type.GetElementType() : type.GetGenericArguments()[0];


            if (!StructParameter.SupportedTypes.ContainsKey(elementType)) return null;

            var arrayAttribute = propertyInfo.GetCustomAttribute<ArrayAttribute>();
            if (arrayAttribute == null)
                throw new ArgumentException($"Property {propertyInfo.DeclaringType.FullName}.{propertyInfo.Name} must have an ArrayAttribute to be used as an array parameter.");
            if (arrayAttribute.Size < 0)
                throw new ArgumentException($"Property {propertyInfo.DeclaringType.FullName}.{propertyInfo.Name} must have a non-negative size in ArrayAttribute.");

            var isDynamicArray = arrayAttribute is DynamicArrayAttribute;
            var size = arrayAttribute.Size;

            return new ArrayParameter(propertyInfo, elementType, isDynamicArray, size);

        } 
    }
}