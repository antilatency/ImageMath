using System;
using System.Linq;
using System.Reflection;

using UnityEngine;
#nullable enable
namespace ImageMath{



    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class MulticompileOptionsAttribute : Attribute {
        public int Min { get; set; }
        public int Max { get; set; }
        public MulticompileOptionsAttribute(int min = 0, int max = 0) {
            Min = min;
            Max = max;
        }
    }

    public static class MulticompileOptionsExtensions {
        public static bool IsMulticompileOptions(this PropertyInfo propertyInfo) {
            return propertyInfo.GetCustomAttribute<MulticompileOptionsAttribute>() != null;
        }
    }

    public class MulticompileOptions {
        protected PropertyInfo _propertyInfo;
        public MulticompileOptions(PropertyInfo propertyInfo) {
            _propertyInfo = propertyInfo;
        }

        public string GetNameList() {
            if (_propertyInfo.PropertyType.IsEnum) {
                var enumNames = Enum.GetNames(_propertyInfo.PropertyType).Select(x=>$"{_propertyInfo.Name}_{x}").ToList();
                return string.Join(" ", enumNames);
            }
            if (_propertyInfo.PropertyType == typeof(bool)) {
                return $"_ {_propertyInfo.Name}";
            }
            if (_propertyInfo.PropertyType == typeof(int)) {
                var attribute = _propertyInfo.GetCustomAttribute<MulticompileOptionsAttribute>()!;
                if (attribute.Min < 0 || attribute.Max <= attribute.Min) {
                    string error = $"For integer properties, MulticompileOptionsAttribute must have valid Min and Max values. Property: {_propertyInfo.DeclaringType.FullName}.{_propertyInfo.Name}";
                    Debug.LogError(error);
                    return $"//Error: {error}";
                }
                return string.Join(" ", Enumerable.Range(attribute.Min, attribute.Max - attribute.Min + 1).Select(i => $"{_propertyInfo.Name}_{i}"));
            }

            {
                var error = $"Unsupported type for multicompile options: {_propertyInfo.PropertyType} {_propertyInfo.Name}";
                Debug.LogError(error);
                return $"//Error: {error}";
            }
       }

        public string GetShaderParameterAssignmentCode() {
            if (_propertyInfo.PropertyType.IsEnum) {
                return $"SetEnumKeyword(\"{_propertyInfo.Name}\", {_propertyInfo.Name});";
            }
            if (_propertyInfo.PropertyType == typeof(bool)) {
                return $"if ({_propertyInfo.Name}) EnableKeyword(\"{_propertyInfo.Name}\"); else DisableKeyword(\"{_propertyInfo.Name}\");";
            }
            if (_propertyInfo.PropertyType == typeof(int)) {
                var attribute = _propertyInfo.GetCustomAttribute<MulticompileOptionsAttribute>()!;
                if (attribute.Max <= attribute.Min) {
                    string error = $"For integer properties, MulticompileOptionsAttribute must have valid Min and Max values. Property: {_propertyInfo.DeclaringType.FullName}.{_propertyInfo.Name}";
                    Debug.LogError(error);
                    return $"//Error: {error}";
                }
                return $"SetEnumKeyword(\"{_propertyInfo.Name}\", {_propertyInfo.Name}, {attribute.Min}, {attribute.Max});";
            }
            {
                var error = $"Unsupported type for multicompile options: {_propertyInfo.PropertyType} {_propertyInfo.Name}";
                Debug.LogError(error);
                return $"//Error: {error}";
            }
        }
    }

    public abstract class Parameter {

        //public abstract string GetPrefix();

        public string GetShaderVariableName() => _propertyInfo.Name;//$"ImageMath_{GetPrefix()}{Index}";
        //public string GetDefine() => "//no define"; //$"#define {_propertyInfo.Name} {GetShaderVariableName()}";

        public abstract string GetShaderParameterAssignmentCode();
        public abstract string GetHLSLDeclaration();

        //public int Index = 0;
        protected PropertyInfo _propertyInfo;

        public Parameter(PropertyInfo propertyInfo) {
            _propertyInfo = propertyInfo;
        }

        public static Parameter? Create(PropertyInfo propertyInfo) {
            Parameter? result = null;

            result = StructParameter.Create(propertyInfo);
            if (result != null) return result;

            result = TextureParameter.Create(propertyInfo);
            if (result != null) return result;

            result = ArrayParameter.Create(propertyInfo);
            return result;
        }
    }
}