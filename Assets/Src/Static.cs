using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using Cache;

namespace ImageMath {

    public static partial class Static {


        /*private static RenderTextureDescriptor DefaultRenderTextureDescriptor => new RenderTextureDescriptor(0, 0) {
            autoGenerateMips = false,
            useMipMap = false,
        };*/


        private static RenderTextureDescriptor GetDefaultRenderTextureDescriptor(int width, int height, bool useMipMap, RenderTextureFormat format) {
            var descriptor = new RenderTextureDescriptor(width, height) {
                autoGenerateMips = false,
                useMipMap = useMipMap,
                colorFormat = format
            };
            return descriptor;
        }



        public static void Add(this RenderTexture _this, Texture increment) {
            new TextureCopy {
                Texture = increment
            }.AddTo(_this);
        }
        public static void AddRGB(this RenderTexture _this, Texture increment) {
            new TextureCopy {
                ChannelMask = ChannelMask.RGB,
                Texture = increment
            }.AddTo(_this);
        }
        public static void MultiplyRGB(this RenderTexture _this, float multiplier){
            _this.MultiplyRGB(Vector3.one * multiplier);
        }
        
        public static void MultiplyRGB(this RenderTexture _this, Vector3 multiplier) {
            new ColorFill() {
                ChannelMask = ChannelMask.RGB,
                Color = multiplier
            }.MultiplyTo(_this);
        }
        
        public static void Substruct(this RenderTexture _this, Texture right) {
            new TextureMultipliedByVector {
                Texture = right,
                Multiplier = -Vector4.one,
            }.AddTo(_this);
        }
        public static void SubstructRGB(this RenderTexture _this, Texture right) {
            new TextureMultipliedByVector {
                ChannelMask = ChannelMask.RGB,
                Texture = right,
                Multiplier = -Vector3.one,
            }.AddTo(_this);
        }




        public static void Assign(this RenderTexture _this, Texture source) {
            new TextureCopy {
                Texture = source
            }.AssignTo(_this);
        }


        public static CacheItem<RenderTexture> GetTempRenderTexture(
            Vector2Int size,
            [CallerFilePath] string file = "",
            [CallerLineNumber] int line = 0
        ) => GetTempRenderTexture(size.x, size.y, false, FilterMode.Bilinear, RenderTextureFormat.ARGBFloat, file, line);

        public static CacheItem<RenderTexture> GetTempRenderTexture(Texture texture,
            [CallerFilePath] string file = "",
            [CallerLineNumber] int line = 0) {
            return GetTempRenderTexture(texture.width, texture.height, false, FilterMode.Bilinear, RenderTextureFormat.ARGBFloat, file, line);
        }


        public static CacheItem<RenderTexture> GetTempRenderTexture(
        int width, int height = 0, bool useMipMap = false, FilterMode filterMode = FilterMode.Bilinear, RenderTextureFormat format = RenderTextureFormat.ARGBFloat,

        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0
        ) {
            if (height <= 0) height = width;
            var description = new {
                Height = height,
                Width = width,
                UseMipMap = useMipMap,
                Format = format
            };
            var item = Cache.Static.GetByDescription<RenderTexture>(description);
            if (item == null) {
                item = new CacheItem<RenderTexture> {
                    Description = description,
                    Value = new RenderTexture(GetDefaultRenderTextureDescriptor(width, height, useMipMap, format)),
                    Destructor = x => {
                        UnityEngine.Object.DestroyImmediate(x);
                    },
                    Validator = x => x
                };
                Cache.Static.Add(item);
            }
            item.Acquire(file, line);
            item.Value.filterMode = filterMode;
            return item;
        }


        

        public static CacheItem<Texture2D> GetTempTexture2DFloat4(
            int width, int height = 0,

            [CallerFilePath] string file = "",
            [CallerLineNumber] int line = 0
        ) {
            if (height <= 0) height = width;
            var description = new {
                Height = height,
                Width = width,
            };
            var item = Cache.Static.GetByDescription<Texture2D>(description);
            if (item == null) {
                item = new CacheItem<Texture2D> {
                    Description = description,
                    Value = CreateFloat4Texture2D(width,height),
                    Destructor = x => {
                        UnityEngine.Object.DestroyImmediate(x);
                    },
                    Validator = x => x
                };
                Cache.Static.Add(item);
            }
            item.Acquire(file, line);
            return item;
        }


        public static RenderTexture CreateRenderTexture(Texture texture, bool useMipMap = false, RenderTextureFormat format = RenderTextureFormat.ARGBFloat) {
            return CreateRenderTexture(texture.width, texture.height, useMipMap, format);
        }
        public static RenderTexture CreateRenderTexture(Vector2Int size, bool useMipMap = false, RenderTextureFormat format = RenderTextureFormat.ARGBFloat) {
            return CreateRenderTexture(size.x, size.y, useMipMap, format);
        }
        public static RenderTexture CreateRenderTexture(int width, int height = 0, bool useMipMap = false, RenderTextureFormat format = RenderTextureFormat.ARGBFloat) {
            if (height <= 0) height = width;
            return new RenderTexture(GetDefaultRenderTextureDescriptor(width, height, useMipMap, format));
        }



        public static Color32[] GetPixels32(this Texture texture) {
            Color32[] pixels;
            if (texture is Texture2D texture2D) {
                pixels = texture2D.GetPixels32();
            } else
            if (texture is RenderTexture renderTexture) {
                pixels = renderTexture.GetPixels32();
            } else
            if (texture is WebCamTexture webCamTexture){
                pixels = webCamTexture.GetPixels32();
            } else {
                throw new NotImplementedException();
            }            
            return pixels;
        }

        public static Color32[] GetPixels32(this RenderTexture renderTexture) {
            var texture = renderTexture.ToTexture();
            var pixels = texture.GetPixels32();
            UnityEngine.Object.DestroyImmediate(texture);
            return pixels;
        }

        public static Color[] GetPixels(this Texture texture) {
            Color[] pixels;
            if (texture is Texture2D texture2D) {
                pixels = texture2D.GetPixels();
            } else
            if (texture is RenderTexture renderTexture) {
                pixels = renderTexture.GetPixels();
            } else
            if (texture is WebCamTexture webCamTexture) {
                pixels = webCamTexture.GetPixels();
            } else {
                throw new NotImplementedException();
            }
            return pixels;
        }

        public static Color[] GetPixels(this RenderTexture renderTexture) {
            //FIXME: use cache
            using var texture = renderTexture.ToTempTexture();
            var pixels = texture.Value.GetPixels();
            return pixels;
        }

        public static Vector4[] GetRawTextureData(this Texture texture) {
            Vector4[] pixels;
            if (texture is Texture2D texture2D) {
                pixels = texture2D.GetRawTextureData<Vector4>().ToArray();
            } else
            if (texture is RenderTexture renderTexture) {
                pixels = renderTexture.GetRawTextureData();
            } else {
                throw new NotImplementedException();
            }
            return pixels;
        }

        public static Vector4[] GetRawTextureData(this RenderTexture renderTexture) {             
            using var texture = renderTexture.ToTempTexture();
            var pixels = texture.Value.GetRawTextureData<Vector4>().ToArray();
            return pixels;
        }

        public static Texture2D ToTexture(this RenderTexture renderTexture, bool apply = false) {
            var texture = new Texture2D(renderTexture.width, renderTexture.height, renderTexture.graphicsFormat, TextureCreationFlags.None);
            renderTexture.ToTexture(texture, apply);
            return texture;
        }

        public static CacheItem<Texture2D> ToTempTexture(this RenderTexture renderTexture, bool apply = false) {
            var item = GetTempTexture2DFloat4(renderTexture.width, renderTexture.height);
            renderTexture.ToTexture(item.Value, apply);
            return item;
        }
            

        public static Texture2D ToTexture(this RenderTexture renderTexture, Texture2D texture, bool apply = false) {
            if (texture == null) {
                return renderTexture.ToTexture(apply);
            }
            RenderTexture currentRT = RenderTexture.active;
            RenderTexture.active = renderTexture;
            texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            if (apply)
                texture.Apply();
            RenderTexture.active = currentRT;
            return texture;
        }


        /*public static void ToTexture(this RenderTexture renderTexture, Texture texture, bool apply = false) {
             if (texture == null)
                 texture = renderTexture.ToTexture(apply);
             else {
                 if (texture is Texture2D texture2D) {
                     renderTexture.ToTexture(texture2D, apply);
                 } else {
                     throw new NotImplementedException();                
                 }
             }                
         }*/

        public static async Task SavePNGAsync(this Texture texture, string filePath) {
            filePath = Path.ChangeExtension(filePath, ".png");
            await File.WriteAllBytesAsync(filePath, texture.SavePNG());
        }


        public static void SavePNG(this Texture texture, string filePath) {
            filePath = Path.ChangeExtension(filePath, ".png");
            File.WriteAllBytes(filePath, texture.SavePNG());
        }

        public static byte[] SavePNG(this Texture texture) {
            if (texture is Texture2D texture2D) {
                return texture2D.EncodeToPNG();
            }
            if (texture is RenderTexture renderTexture) {
                var t = renderTexture.ToTexture();
                return t.EncodeToPNG();
            }
            throw new NotImplementedException();
        }

        public static byte[] SaveEXR(this Texture texture) {
            if (texture is Texture2D texture2D) {
                return texture2D.EncodeToEXR();
            }
            if (texture is RenderTexture renderTexture) {
                var t = renderTexture.ToTexture();
                return t.EncodeToEXR();
            }
            throw new NotImplementedException();
        }


        public static string SimpleFormatExtension = ".f4a2d";

        public static void ClearAlpha(this RenderTexture renderTexture, float value = 1) {
            new ColorFill() {
                ChannelMask = ChannelMask.A,
                Color = new Vector4(0, 0, 0, value)
            }.AssignTo(renderTexture);
        }

        public static async Task SaveSimpleFormatAsync(this Texture texture, string filePath) {
            filePath = Path.ChangeExtension(filePath, SimpleFormatExtension);
            await File.WriteAllBytesAsync(filePath, texture.SaveSimpleFormat());
        }

        public static void SaveSimpleFormat(this Texture texture, string filePath) {
            filePath = Path.ChangeExtension(filePath, SimpleFormatExtension);
            File.WriteAllBytes(filePath, texture.SaveSimpleFormat());
        }

        public static byte[] SaveSimpleFormat(this Texture texture) {
            var width = texture.width;
            Color[] pixels = texture.GetPixels();
            return SaveSimpleFormat(width, pixels);
        }

        public static byte[] SaveSimpleFormat<T>(int width, T[] pixels) {
            var pixelsSize = pixels.Length * 4 * sizeof(float);
            var widthSize = sizeof(int);
            var result = new byte[widthSize + pixelsSize];

            BitConverter.GetBytes(width).CopyTo(result, 0);

            GCHandle handle = GCHandle.Alloc(pixels, GCHandleType.Pinned);
            try {
                Marshal.Copy(handle.AddrOfPinnedObject(), result, widthSize, pixelsSize);
            }
            finally {
                handle.Free();
            }
            return result;
        }

        public static async Task<Texture2D> LoadSimpleFormatAsync(string filePath, bool apply = true) {
            filePath = Path.ChangeExtension(filePath, SimpleFormatExtension);
            var data = await File.ReadAllBytesAsync(filePath);
            return LoadSimpleFormat(data, apply);
        }

        public static Texture2D LoadSimpleFormat(string filePath, bool apply = true) {
            filePath = Path.ChangeExtension(filePath, SimpleFormatExtension);
            var data = File.ReadAllBytes(filePath);
            return LoadSimpleFormat(data, apply);
        }


        public static Texture2D LoadSimpleFormat(byte[] data, bool apply = true) {
            var textureData = ReadSimpleFormat<Color>(data);
            return CreateFloat4Texture2D(textureData.width, textureData.pixels, apply);
        }


        public static (int width, Color[] pixels) ReadSimpleFormat(byte[] data) {
            return ReadSimpleFormat<Color>(data);
        }

        public static Texture2D CreateFloat4Texture2D(int width, int height) {
            Texture2D texture = new Texture2D(width, height, GraphicsFormat.R32G32B32A32_SFloat, TextureCreationFlags.None);
            texture.anisoLevel = 0;
            return texture;
        }

        public static Texture2D CreateFloat4Texture2D(int width, Color[] pixels, bool apply = true) {
            var height = pixels.Length / width;
            Texture2D texture = CreateFloat4Texture2D(width, height);
            texture.SetPixelData(pixels,0,0);
            if (apply)
                texture.Apply();
            return texture;
        }

        public static (int width, T[] pixels) ReadSimpleFormat<T>(byte[] data) {
            if (data == null || data.Length < sizeof(int)) {
                throw new Exception("Invalid data: The input data is either null or too short to contain the width information.");
            }

            int width = BitConverter.ToInt32(data, 0);

            var pixelSize = 4 * sizeof(float);

            int pixelsDataLength = data.Length - sizeof(int);

            if (pixelsDataLength % pixelSize != 0) {
                throw new Exception("Invalid data: The length of the pixel data is not a multiple of the expected pixel size.");
            }

            int pixelCount = pixelsDataLength / pixelSize;

            if (pixelCount % width != 0) {
                throw new Exception("Invalid data: The pixel count is not a multiple of the provided width.");
            }

            T[] pixels = new T[pixelCount];

            GCHandle handle = GCHandle.Alloc(pixels, GCHandleType.Pinned);
            try {
                Marshal.Copy(data, sizeof(int), handle.AddrOfPinnedObject(), pixelsDataLength);
            }
            finally {
                handle.Free();
            }

            return (width, pixels);
        }

        public static Color GetPixelBilinearCorrect(this Texture2D texture, Vector2 uv) {
            var offsetX = 0.5f / texture.width;
            var offsetY = 0.5f / texture.height;
            var color = texture.GetPixelBilinear(uv.x - offsetX, uv.y - offsetY);
            return color;
        }


        public static Color GetPixelBilinearPixelPosition(this Texture2D texture, Vector2 pixelPosition) {
            var uv = new Vector2(
                (pixelPosition.x-0.5f) / texture.width,
                (pixelPosition.y-0.5f) / texture.height
            );
            return texture.GetPixelBilinear(uv.x, uv.y);
        }

        public static ComputeBuffer CreateComputeBuffer(ComputeBufferParameters parameters) {
            return new ComputeBuffer(parameters.Count, parameters.Stride, parameters.Type);
        }


        public static CacheItem<ComputeBuffer> GetTempComputeBuffer(ComputeBufferParameters parameters, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) {

            var item = Cache.Static.GetByDescription<ComputeBuffer>(parameters);
            if (item == null) {
                item = new CacheItem<ComputeBuffer> {
                    Description = parameters,
                    Value = CreateComputeBuffer(parameters),
                    Destructor = x => {
                        x.Dispose();
                    },
                    Validator = x => x.IsValid()
                };
                Cache.Static.Add(item);
            }
            item.Acquire(file, line);
            return item;
        }


    }

}
