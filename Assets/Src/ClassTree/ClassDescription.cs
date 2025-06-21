using System;
using System.Collections.Generic;

using System.Linq;
#nullable enable
namespace ImageMath{
    public class ClassDescription{
        public Type Type { get; set; }
        public string Name => Type.Name;
        public string Namespace => Type.Namespace;
        public List<ClassDescription> Children { get; set; } = new List<ClassDescription>();
        public ClassDescription? Parent { get; private set; } = null;

        //public Dictionary<string,int> ParametersIndices { get; set; } = new Dictionary<string,int>();

        public List<Parameter> Parameters = new();
        public List<MulticompileOptions> MulticompileOptionsList = new();

        public ClassDescription(Type type, ClassDescription? parent) {
            Type = type;
            Parent = parent;

            var properties = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);
            foreach (var property in properties) {
                if (property.CanRead && !property.IsMulticompileOptions()) {
                    var parameter = Parameter.Create(property);
                    if (parameter != null) {
                        Parameters.Add(parameter);
                    }
                }
            }
            MulticompileOptionsList = properties
                .Where(p => p.IsMulticompileOptions())
                .Select(p => new MulticompileOptions(p))
                .ToList();

        }

        private static IEnumerable<Type> HierarchyTo(Type type, Type last){
            do{
                yield return type;
                if (type == last){
                    yield break;
                }
                type = type.BaseType;
                if (type == null){
                    break;
                }
            } while(true);
        }

        public ClassDescription FindOrCreate(Type type){
            var hierarchy = HierarchyTo(type, Type);

            var last = hierarchy.Last();
            if (last != Type){
                throw new InvalidOperationException($"Type {last} does not match {Type}");
            }
            hierarchy = hierarchy.SkipLast(1);
            if (!hierarchy.Any()){
                return this;
            }
            var childType = hierarchy.Last();
            var child = Children.FirstOrDefault(c => c.Type == childType);
            if (child == null){
                child = new ClassDescription(childType, this);
                Children.Add(child);
            }
            return child.FindOrCreate(type);
        }

        public void FlattenChildren(List<ClassDescription> list){
            foreach (var child in Children){
                list.Add(child);
                child.FlattenChildren(list);
            }
        }

        public override string ToString(){
            return Type.Name;
        }

        public IEnumerable<Type> GetHierarchy(){
            var classDescription = this;
            while (classDescription != null){
                yield return classDescription.Type;
                classDescription = classDescription.Parent;
            }
        }
    }
}