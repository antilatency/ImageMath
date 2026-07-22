using System;

#nullable enable

namespace ImageMath{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class TextureFormatAttribute : Attribute {
        public enum ElementType {
            Float,
            Half,
            UInt,
            Int
        }

        public ElementType Type { get; protected set;}
        public int Size { get; protected set;}

        public TextureFormatAttribute(ElementType type, int size) {
            Type = type;
            Size = size;
            if (size <1 || size > 4) {
                throw new ArgumentException("Size must be between 1 and 4");
            }
        }

        public string GetHLSLType() {
            var elementTypeName = Type switch {
                ElementType.Float => "float",
                ElementType.Half => "half",
                ElementType.UInt => "uint",
                ElementType.Int => "int",
                _ => throw new ArgumentException("Invalid element type")
            };
            var vectorTypeName = Size switch {
                1 => elementTypeName,
                2 => $"{elementTypeName}2",
                3 => $"{elementTypeName}3",
                4 => $"{elementTypeName}4",
                _ => throw new ArgumentException("Size must be between 1 and 4")
            };

            return vectorTypeName;
        }
    }
}
