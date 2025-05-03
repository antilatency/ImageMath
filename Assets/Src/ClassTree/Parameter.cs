using System.Reflection;
#nullable enable
namespace ImageMath{
    public abstract class Parameter {
        
        public abstract string GetPrefix();

        public string GetShaderVariableName() => $"ImageMath_{GetPrefix()}{Index}";
        public string GetDefine() => $"#define {field.Name} {GetShaderVariableName()}";

        public abstract string GetShaderParameterAssignmentCode();
        public abstract string GetHLSLDeclaration();

        public int Index = 0;
        protected FieldInfo field;

        public Parameter(FieldInfo field) {
            this.field = field;
        }

        public static Parameter? Create(FieldInfo field) {
            Parameter? result = null;
            result = VectorParameter.Create(field);
            if (result != null) return result;
            result = ScalarParameter.Create(field);
            if (result != null) return result;
            result = TextureParameter.Create(field);
            if (result != null) return result;
            result = MatrixParameter.Create(field);
            return result;
        }
    }
}