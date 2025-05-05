using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
#nullable enable
namespace ImageMath.Views{
    [ExecuteAlways]
    public class TextureView : MonoBehaviour {

        private Texture? CreatedTexture = null;
        public Texture? Texture  = null;

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


        public RenderTexture ResizeRenderTexture(int width, int height = 0, bool useMipMap = false) {
            if (height == 0) height = width;
            if (Texture) {
                if (Texture is RenderTexture renderTexture)
                    if (Texture.width == width)
                        if (Texture.height == height)
                            if ((Texture.mipmapCount>1) == useMipMap)
                                return renderTexture;
                if (CreatedTexture)
                    DestroyImmediate(CreatedTexture);
            }

            CreatedTexture = ImageMath.Static.CreateRenderTexture(width, height,useMipMap);
            Texture = CreatedTexture;
            return Texture as RenderTexture;
        }

        public static TextureView GetByName(string name){
            var found = GameObject.FindObjectsOfType<TextureView>(true).FirstOrDefault(x => x.name == name);
            if (found == null) {
                found = new GameObject(name).AddComponent<TextureView>();
            }
            return found;
        }
    }
}