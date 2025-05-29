
using ImageMath;
using ImageMath.Views;
using UnityEngine;

[ExecuteAlways]
public class BasicTests : MonoBehaviour {

    public Vector3 LightDirection = Vector3.forward;
    public Color LightColor = Color.white;

    [Range(0, 2)]
    public float LineSoftness = 0.5f;

    [Range(2, 16)]
    public int ReductorDownScalePerIteration = 2;
    public Vector4 AverageValue;
    public Vector4 MaxValue;
    public Vector4 MinValue;



    void Update() {
        var inputTexture = TextureView.GetByName("Input").ResizeRenderTexture(512, 512);
        new UVFill() {
        }.AssignTo(inputTexture);

        new EllipseFill(new Vector4(1, 0, 0, 0.5f), Vector4.zero)
        .Tile(2, new Vector2Int(2, 2))
        .AlphaBlendTo(inputTexture);

        new EllipseFill(new Vector4(0.5f, 0.5f, 0, 1), Vector4.one) {
            ChannelMask = ChannelMask.RGB,
            Radius = new Vector2(0.25f, 0.25f),
        }.MinTo(inputTexture);


        var lambertianSphereTexture = TextureView.GetByName("LambertianSphere").ResizeRenderTexture(512, 512);


        new LambertianSphereFill(LightDirection, LightColor.linear.ToVector3()).AssignTo(lambertianSphereTexture);

        var lineTextureView = TextureView.GetByName("Lines");
        var pointA = lineTextureView.transform.GetChild(0).localPosition;
        var pointB = lineTextureView.transform.GetChild(1).localPosition;
        var linesTexture = lineTextureView.ResizeRenderTexture(512, 512);
        new LineFill() {
            Color = new Vector4(1, 1, 1, 1),
            PointA = pointA,
            PointB = pointB,
            LineWidth = 0.1f,
            LineSoftness = LineSoftness,
        }
        .AssignTo(linesTexture);



        var reductionInput = TextureView.GetByName("ReductionInput").ResizeRenderTexture(8192, 8192);
        new UVFill() {
        }.AssignTo(reductionInput);
        new ColorFill(new Vector4(0, 0, 0.25f, 0)).AddTo(reductionInput);

        /*var pixels = new Vector4[TestTexture2D.width * TestTexture2D.height];

        pixels[PixelIndex.x + PixelIndex.y * TestTexture2D.width] = new Vector4(1/3.0f, 0, 0, 1);
        TestTexture2D.SetPixelData(pixels, 0);
        TestTexture2D.Apply();*/


        /*var reductionResult = TextureView.GetByName("ReductionResult").ResizeRenderTexture(1333/2,1333/2);
        reductionResult.filterMode = FilterMode.Point;
        new AverageWeightedByAlphaOperation(TestTexture2D){
        }.AssignTo(reductionResult);*/

        AverageValue = reductionInput.AverageWeightedByAlpha(ReductorDownScalePerIteration);
        MaxValue = reductionInput.Maximum(ReductorDownScalePerIteration);
        MinValue = reductionInput.Minimum(ReductorDownScalePerIteration);

        var textureCompareInput = TextureView.GetByName("TextureCompareInput").ResizeRenderTexture(128, 128);
        new UVFill() {
        }.AssignTo(textureCompareInput);
        new GradientFill {
            PointA = new Vector2(0.5f, 0.5f),
            PointB = new Vector2(1, 1),
            ColorA = new Vector4(0, 0, 0, 1),
            ColorB = new Vector4(0, 0, 1, 1),
            ChannelMask = ChannelMask.B
        }.AssignTo(textureCompareInput);

        var textureCompareResult = TextureView.GetByName("TextureCompareResult").ResizeRenderTexture(128, 128);
        new TextureCompare(textureCompareInput, TextureCompare.CompareOperation.Equal, 0.0f)
        .AssignTo(textureCompareResult);

        textureCompareResult.ClearAlpha();

        TransparencyInfillTest();



        //shrink grow test
        /*var shrinkGrowInput = TextureView.GetByName("ShrinkGrowInput").Texture;
        if (shrinkGrowInput != null) {

            var shrinkGrowResult = TextureView.GetByName("ShrinkGrowResult").ResizeRenderTexture(512, 512);
            new ShrinkGrow(shrinkGrowInput, 0.5f, 0.5f, 0.5f, 0.5f) {
                ChannelMask = ChannelMask.RGB,
            }.AssignTo(shrinkGrowResult);
        }*/



    }
    [Range(0, 1)]
    public float TransparencyInfillPower = 1.0f;

    void TransparencyInfillTest() {
        var inputTexture = TextureView.GetByName("TransparencyInfillInput").ResizeRenderTexture(512, 512,true);
        new UVFill(){
        }.AssignTo(inputTexture);

        new EllipseFill(new Vector4(0, 0, 0, 0), Vector4.one, null, Vector2.one * 0.25f)
        .MultiplyTo(inputTexture);

        inputTexture.GenerateMips();

        var outputTexture = TextureView.GetByName("TransparencyInfillResult").ResizeRenderTexture(512, 512);


        new TransparencyInfill(inputTexture){
            Power = TransparencyInfillPower,
        }.AssignTo(outputTexture);

        outputTexture.ClearAlpha();

    }

    /*public bool[] InputArray = new bool[10];
    public int[] OutputArray = new int[3];
    public int Sum;

    int GetSumValue(int[] input, int N, int M, int outputIndex) {
        int baseBlockSize = N / M;
        int remainder = N % M;

        int startIndex = outputIndex * baseBlockSize + System.Math.Min(outputIndex, remainder);

        int blockSize = baseBlockSize + (outputIndex < remainder ? 1 : 0);

        int sum = 0;
        for (int i = 0; i < blockSize; i++) {
            sum += input[startIndex + i];
        }

        return sum;
    }


    public void TestReduction() {
        int N = InputArray.Length;
        int M = OutputArray.Length;
        int baseBlockSize = N / M;
        int remainder = N % M;

        var inputAsInts = InputArray.Select(x => x ? 1 : 0).ToArray();


        for (int i = 0; i < M; i++) {

            OutputArray[i] = GetSumValue(inputAsInts, N, M, i) ;;
        }

        Sum = OutputArray.Sum();
    }*/


}
