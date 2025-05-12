using System.Reflection;
#nullable enable
namespace ImageMath{
    public abstract class Parameter {
        
        public abstract string GetPrefix();

        public string GetShaderVariableName() => $"ImageMath_{GetPrefix()}{Index}";
        public string GetDefine() => $"#define {_propertyInfo.Name} {GetShaderVariableName()}";

        public abstract string GetShaderParameterAssignmentCode();
        public abstract string GetHLSLDeclaration();

        public int Index = 0;
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