using ImageMath;
using UnityEngine;


public abstract partial record TestOperationBase : FragmentOperation{
    public Vector3 VectorValue0;
}

public partial record TestOperation : TestOperationBase{
    public int IntValue1;
    public int IntValue2;
    public Vector3 VectorValue1;
    public Vector3 VectorValue2;

    static string GetFragmentShaderBody(ClassDescription classDescription) {
        return "return 0;";
    }
}
