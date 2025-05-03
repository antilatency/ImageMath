using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Cache{

    [ExecuteAlways]
    public class CacheComponent : MonoBehaviour{

        public int NumItems;

        void Update(){
            NumItems = Static.Items.Count;
            Static.Tick();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos(){
            Handles.matrix = transform.localToWorldMatrix;

            var text = new StringBuilder();
            foreach (var i in Static.Items){
                text.AppendLine($"{i.ValueType.Name} {i.File} {i.Line} {i.Description}");
            }

            Handles.Label(Vector3.zero, $"NumItems {NumItems}\n{text.ToString()}");

            Handles.matrix = Matrix4x4.identity;


        }
#endif
    }
}