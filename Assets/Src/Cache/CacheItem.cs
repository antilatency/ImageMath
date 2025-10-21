using System;

namespace Cache{


    public abstract class CacheItem : IDisposable{

        public string File{ get; private set; } = "";
        public int Line{ get; private set; } = 0;

        public abstract Type ValueType{ get; }

        public int Generation = 0;
        public bool Acquired{ get; private set; } = false;

        public abstract bool Valid{ get; }
        public object Description{ get; init; }

        public void Acquire(string file, int line){
            File = file;
            Line = line;
            Acquired = true;
        }

        public void Dispose(){
            Acquired = false;
            Generation = 0;
        }

        public abstract void DestroyValue();
    }

    public class CacheItem<T> : CacheItem{
        public T Value{ get; set; }
        public override Type ValueType => Value.GetType();
        public override bool Valid => Validator == null ? true : Validator(Value);
        public Func<T, bool> Validator{ private get; init; }
        public Action<T> Destructor{ get; init; }

        public override void DestroyValue(){
            Destructor(Value);
            Value = default;
        }

        public static implicit operator T(CacheItem<T> _this){
            return _this.Value;
        }
    }
}
