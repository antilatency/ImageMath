using System;

#nullable enable

namespace ImageMath{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public abstract class ArrayAttribute : Attribute {
        public int Size { get; protected set;}
    }

    public class FixedArrayAttribute : ArrayAttribute {
        public FixedArrayAttribute(int size) {
            Size = size;
        }
    }

    public class DynamicArrayAttribute : ArrayAttribute {
        public DynamicArrayAttribute(int maxSize) {
            Size = maxSize;
        }
    }
}