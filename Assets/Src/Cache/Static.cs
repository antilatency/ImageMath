using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cache{
    public static partial class Static{

        public static int MaxGeneration = 3;
        public static List<CacheItem> Items = new();

        public static void Tick(){
            var newItems = new List<CacheItem>();
            foreach (var item in Items){
                if (item.Acquired){
                    Debug.LogError($"Item still acquired. Was acquired in {item.File}:{item.Line}");
                    //throw new InvalidOperationException($"Item still acquired. Was acquired in {item.File}:{item.Line}");
                }

                item.Generation++;
                if (item.Generation > MaxGeneration || !item.Valid){
                    item.DestroyValue();
                }
                else{
                    newItems.Add(item);
                }
            }

            Items = newItems;
        }

        public static CacheItem<T> GetByDescription<T>(object description){
            LightTick();
            return Items.OfType<CacheItem<T>>().Where(x => !x.Acquired && x.Valid)
                .FirstOrDefault(x => x.Description.Equals(description));
        }

        public static void Add(CacheItem cacheItem){
            Items.Add(cacheItem);
        }

        static int previousFrameCount = -1;
        internal static void LightTick(){
            if (Time.frameCount != previousFrameCount){
                previousFrameCount = Time.frameCount;
                Tick();
            }
        }

    }
}


