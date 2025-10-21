using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.Experimental.Rendering;
using static ImageMath.Static;

#if UNITY_EDITOR
using UnityEditor;
#endif

#nullable enable
namespace ImageMath.Views{
    [ExecuteAlways]
    public class TextureView : MonoBehaviour {

        public enum AlphaBlendingOptions {
            Opaque,
            AlphaBlending,
            PremultipliedAlpha
        }

        private Texture? CreatedTexture = null;
        public Texture? Texture  = null;
        public AlphaBlendingOptions AlphaBlending = AlphaBlendingOptions.Opaque;

        public enum ResizeModeOptions {
            None,
            Width,
            Height,
            Both
        }
        public ResizeModeOptions ResizeMode = ResizeModeOptions.Width;

        public Material? Material = null;
        public static Material? DefaultUIMaterial = null;
        public static Mesh? DefaultMesh = null;

        void UpdateAlphaBlending(Material material) {
            /*
            if (AlphaBlending == AlphaBlendingOptions.Opaque) {
                material.SetInt("_SrcBlend", (int)BlendMode.One);
                material.SetInt("_DstBlend", (int)BlendMode.Zero);
            }*/
            switch (AlphaBlending) {
                case AlphaBlendingOptions.Opaque:
                    material.SetInt("_SrcBlend", (int)BlendMode.One);
                    material.SetInt("_DstBlend", (int)BlendMode.Zero);
                    break;

                case AlphaBlendingOptions.AlphaBlending:
                    material.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
                    break;

                case AlphaBlendingOptions.PremultipliedAlpha:
                    material.SetInt("_SrcBlend", (int)BlendMode.One);
                    material.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
                    break;
            }
        } 

        void UpdateMeshRenderer(){
            

            if (!Texture || Texture==null)
                return;

            var meshRenderer = GetComponent<MeshRenderer>();
            if (!meshRenderer)
                meshRenderer = gameObject.AddComponent<MeshRenderer>();
            var meshFilter = GetComponent<MeshFilter>();
            if (!meshFilter)
                meshFilter = gameObject.AddComponent<MeshFilter>();

            if (DefaultMesh==null){
                DefaultMesh = new Mesh();
                DefaultMesh.vertices = new Vector3[] {
                    new Vector3(0, 0, 0),
                    new Vector3(1, 0, 0),
                    new Vector3(1, 1, 0),
                    new Vector3(0, 1, 0)
                };
                DefaultMesh.uv = new Vector2[] {
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(1, 1),
                    new Vector2(0, 1)
                };
                DefaultMesh.triangles = new int[] { 0, 2, 1, 0, 3, 2 };
            }

            if (!meshFilter.sharedMesh)
                meshFilter.sharedMesh = DefaultMesh;

            meshRenderer.sharedMaterial = GetMaterial(false);
            meshRenderer.sharedMaterial.mainTexture = Texture;

            Vector3 scale = Vector3.one;
            switch (ResizeMode) {
                case ResizeModeOptions.Height:
                    scale.y = (float)Texture.height / Texture.width;                
                    break;

                case ResizeModeOptions.Width:
                    scale.x = (float)Texture.width / Texture.height;
                    break;

                case ResizeModeOptions.Both:
                    scale.x = (float)Texture.width;
                    scale.y = (float)Texture.height;
                    break;
            }
            transform.localScale = scale;

        }

        Material GetMaterial(bool uiMaterial) {
            if (Material == null) {
                if (uiMaterial){
                    if (DefaultUIMaterial == null)
                        DefaultUIMaterial = new Material(Shader.Find("ImageMath/Views/TextureViewUI"));
                    else
                        Material = DefaultUIMaterial;
                } else {
                    Material = new Material(Shader.Find("ImageMath/Views/TextureView"));
                }
            }
            UpdateAlphaBlending(Material!);
            return Material!;
        }

        void UpdateRawImage() {
            var rectTransform = GetComponent<RectTransform>();
            if (!rectTransform)
                return;

            if (!Texture || Texture==null)
                return;

            var size = rectTransform.sizeDelta;
            switch (ResizeMode) {
                case ResizeModeOptions.Height:
                size.y = size.x / Texture.width * Texture.height;
                break;

                case ResizeModeOptions.Width:
                size.x = size.y / Texture.height * Texture.width;
                break;

                case ResizeModeOptions.Both:
                size.x = Texture.width;
                size.y = Texture.height;
                break;
            }
            rectTransform.sizeDelta = size;
            var rawImage = GetComponent<RawImage>();

            if (!rawImage)
                rawImage = gameObject.AddComponent<RawImage>();

            rawImage.texture = Texture;
            rawImage.material = GetMaterial(true);
        }

        public void Update() {


            var canvas = GetComponentInParent<Canvas>();

            if (Texture != CreatedTexture && Texture != null) {
                if (CreatedTexture){
                    DestroyImmediate(CreatedTexture);
                    CreatedTexture = null;
                }
            }

            if (canvas){
                UpdateRawImage();
            } else {
                UpdateMeshRenderer();
            }

            


            
        }


        public Texture2D ResizeTexture2D(int width, int height = 0, bool useMipMap = false, GraphicsFormat graphicsFormat = GraphicsFormat.R32G32B32A32_SFloat) {
            if (height == 0) height = width;
            if (Texture) {
                if (Texture is Texture2D texture2D)
                    if (texture2D.width == width)
                        if (texture2D.height == height)
                            if ((texture2D.mipmapCount > 1) == useMipMap)
                                if (texture2D.graphicsFormat == graphicsFormat)
                                    return texture2D;
                if (CreatedTexture)
                    DestroyImmediate(CreatedTexture);
            }

            var createdTexture = CreateTexture2D(width, height, graphicsFormat, useMipMap);

            CreatedTexture = createdTexture;
            Texture = CreatedTexture;
            return createdTexture;
        }

        public RenderTexture ResizeRenderTexture(int width, int height = 0, bool useMipMap = false, RenderTextureFormat format = RenderTextureFormat.ARGBFloat) {
            if (height == 0) height = width;
            if (Texture) {
                if (Texture is RenderTexture renderTexture)
                    if (Texture.width == width)
                        if (Texture.height == height)
                            if ((Texture.mipmapCount > 1) == useMipMap)
                                if (renderTexture.format == format)
                                    return renderTexture;
                if (CreatedTexture)
                    DestroyImmediate(CreatedTexture);
            }

            var createdRenderTexture = ImageMath.Static.CreateRenderTexture(width, height, useMipMap, format);
            CreatedTexture = createdRenderTexture;
            Texture = CreatedTexture;
            return createdRenderTexture;
        }

        public static TextureView GetByName(string name){
            var found = GameObject.FindObjectsOfType<TextureView>(true).FirstOrDefault(x => x.name == name);
            if (found == null) {
                found = new GameObject(name).AddComponent<TextureView>();
            }
            return found;
        }

        #if UNITY_EDITOR
        [ContextMenu("Save Image As PNG")]
        private void SaveImageAsPNG() {
            SaveImageAsPNG(false,x=>x.SavePNG(),"png");
        }
        [ContextMenu("Save Image As PNG (Pack to sRGB)")]
        private void SaveImageAsPNGPackToSRGB() {
            SaveImageAsPNG(true,x=>x.SavePNG(),"png");
        }

        [ContextMenu("Save Image As EXR")]
        private void SaveImageAsEXR() {
            SaveImageAsPNG(false,x=>x.SaveEXR(),"exr");
        }


        private void SaveImageAsPNG(bool packToSRGB, Func<Texture,byte[]> Encode, string extension) {
            if (Texture == null){
                Debug.LogWarning("No texture assigned.");
                return;
            }

            byte[] pngData;
            if (packToSRGB){
                using var temp = Static.GetTempRenderTexture(Texture.width, Texture.height);
                new PackSRGB(Texture).AssignTo(temp);
                pngData = Encode(temp.Value);                       
            } else {
                pngData = Encode(Texture);
            }

            string path = EditorUtility.SaveFilePanel(
                $"Save Image As {extension.ToUpper()}",
                "",
                gameObject.name + "." + extension,
                extension
            );

            if (!string.IsNullOrEmpty(path)){
                System.IO.File.WriteAllBytes(path, pngData);
                Debug.Log("Image saved to: " + path);
            }
        }
        #endif


    }
}