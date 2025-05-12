using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


#nullable enable
namespace ImageMath {



    [FilePath]
    public abstract partial record RectOperation: MaterialOperation {

        public Vector2 Position {get; set;} = Vector2.zero;
        public Vector2 Size {get; set;} = Vector2.one;

        #if UNITY_EDITOR
        public static new string GetVertexShader(ClassDescription classDescription) => LoadCode("RectOperation.VertexShader.cginc");
        #endif



    }
}
