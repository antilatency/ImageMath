using System.Collections;
using System.Collections.Generic;
using ImageMath;
using ImageMath.Views;
using UnityEngine;

[ExecuteAlways]
public class BasicTests : MonoBehaviour {

    public Vector3 LightDirection = Vector3.forward;
    public Color LightColor = Color.white;

    [Range(0, 2)]
    public float LineSoftness = 0.5f;

    void Update() {
        var inputTexture = TextureView.GetByName("Input").ResizeRenderTexture(512, 512);
        new UVFill(){
        }.AssignTo(inputTexture);

        new EllipseFill(new Vector4(1, 0, 0, 0.5f), Vector4.zero)
        .Tile(2,new Vector2Int(2,2))
        .AlphaBlendTo(inputTexture);

        new EllipseFill(new Vector4(0.5f, 0.5f, 0, 1), Vector4.one){
            ChannelMask = ChannelMask.RGB,
            Radius = new Vector2(0.25f, 0.25f),
        }.MinTo(inputTexture);


        var LambertianSphereTexture = TextureView.GetByName("LambertianSphere").ResizeRenderTexture(512, 512);


        new LambertianSphereFill(LightDirection, LightColor.linear.ToVector3()).AssignTo(LambertianSphereTexture);

        var lineTextureView = TextureView.GetByName("Lines");
        var pointA = lineTextureView.transform.GetChild(0).localPosition;
        var pointB = lineTextureView.transform.GetChild(1).localPosition;
        var linesTexture = lineTextureView.ResizeRenderTexture(512, 512);
        new LineFill(){
            Color = new Vector4(1, 1, 1, 1),
            PointA = pointA,
            PointB = pointB,
            LineWidth = 0.1f,
            LineSoftness = LineSoftness,
        }
        .AssignTo(linesTexture);


    }
}
