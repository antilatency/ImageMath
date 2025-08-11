using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

#if UNITY_EDITOR
#endif
#nullable enable
namespace ImageMath {
    public static class FFMPEGAudioReader{

        [Serializable]
        public class Info{
            public short NumChannels;
            public int SampleRate;
        }
        
        struct FormatChunk{
            public short AudioFormat;
            public short NumChannels;
            public int SampleRate;
            public int BytePerSec;
            public short BytePerBloc;
            public short BitsPerSample;
        }


        public static List<Vector2> ReadAudioSamples(string inputPath, string? parameters = null, Info? info = null, Action<string>? logger = null) {
            var audioParameters = "-vn -f wav -";
            var ffmpegProcess = FFMPEG.Run(inputPath, parameters+" "+audioParameters, logger);
            var stream = ffmpegProcess.StandardOutput.BaseStream;

/*
[Master RIFF chunk]
   FileTypeBlocID  (4 bytes) : Identifier « RIFF »  (0x52, 0x49, 0x46, 0x46)
   FileSize        (4 bytes) : Overall file size minus 8 bytes
   FileFormatID    (4 bytes) : Format = « WAVE »  (0x57, 0x41, 0x56, 0x45)

[Chunk describing the data format]
   FormatBlocID    (4 bytes) : Identifier « fmt␣ »  (0x66, 0x6D, 0x74, 0x20)
   BlocSize        (4 bytes) : Chunk size minus 8 bytes, which is 16 bytes here  (0x10)
   AudioFormat     (2 bytes) : Audio format (1: PCM integer, 3: IEEE 754 float)
   NbrChannels     (2 bytes) : Number of channels
   Frequency       (4 bytes) : Sample rate (in hertz)
   BytePerSec      (4 bytes) : Number of bytes to read per second (Frequency * BytePerBloc).
   BytePerBloc     (2 bytes) : Number of bytes per block (NbrChannels * BitsPerSample / 8).
   BitsPerSample   (2 bytes) : Number of bits per sample

[Chunk containing the sampled data]
   DataBlocID      (4 bytes) : Identifier « data »  (0x64, 0x61, 0x74, 0x61)
   DataSize        (4 bytes) : SampledData size
   SampledData
*/
            var reader = new BinaryReader(stream);
            var riff = reader.ReadInt32();
            var fileSize = reader.ReadInt32();
            var wave = reader.ReadInt32();

            //check if it is a wave file
            if (riff != 0x46464952 || wave != 0x45564157) {
                throw new Exception("Not a wave file");
            }

            var formatChunk = new FormatChunk();

            //read chunks
            while(true){
                var chunkID = Encoding.ASCII.GetString(reader.ReadBytes(4));
                var chunkSize = reader.ReadInt32();

                if (chunkID == "fmt "){
                    formatChunk.AudioFormat = reader.ReadInt16();
                    formatChunk.NumChannels = reader.ReadInt16();   
                    formatChunk.SampleRate = reader.ReadInt32();
                    formatChunk.BytePerSec = reader.ReadInt32();
                    formatChunk.BytePerBloc = reader.ReadInt16();
                    formatChunk.BitsPerSample = reader.ReadInt16();

                    if (info != null) {
                        info.NumChannels = formatChunk.NumChannels;
                        info.SampleRate = formatChunk.SampleRate;
                    }
                }
                else if (chunkID == "data"){
                    
                    
                    if (formatChunk.BitsPerSample == 16 && formatChunk.NumChannels == 2 & formatChunk.AudioFormat == 1){
                        List<Vector2> samples = new();
                        
                        var buffer = new byte[4096];

                        while (true) {
                            var bytesRead = stream.Read(buffer, 0, buffer.Length);
                            if (bytesRead == 0)
                                return samples;

                            for (int i = 0; i < bytesRead; i += 4) {
                                if (i + 3 >= bytesRead){
                                    throw new Exception("Invalid data size");
                                }

                                var left = BitConverter.ToInt16(buffer, i) / (float)short.MaxValue;
                                var right = BitConverter.ToInt16(buffer, i + 2) / (float)short.MaxValue;
                                samples.Add(new Vector2(left, right));
                            }
                        }        
                    }
                    else{
                        throw new Exception($"Unsupported data format: {formatChunk.BitsPerSample} bits per sample, {formatChunk.NumChannels} channels, audio format {(formatChunk.AudioFormat==0?"PCM":"IEEE 754 float")}");
                    }
                }
                else{
                    reader.ReadBytes(chunkSize);
                }
            }
        }
    }




}
