using System.Collections.Generic;
using System.Diagnostics;
using System;
using System.IO;
using UnityEngine;


#nullable enable
namespace ImageMath {



    public class FFMPEGImageReader : IDisposable {
        string _inputPath;
        Action<string>? _logger;

        MediaInfo _mediaInfo;
        StreamInfo _videoStreamInfo;
        

        public int Width => _videoStreamInfo.Width;
        public int Height => _videoStreamInfo.Height;

        public float FPS => _videoStreamInfo.NumberOfFrames / _videoStreamInfo.Duration;

        public int NumberOfFrames => _videoStreamInfo.NumberOfFrames;

        public float Duration => _videoStreamInfo.Duration;

        Process? ffmpegProcess;
        IEnumerator<ReadFrameStatus>? readFrameEnumerator;

        public readonly byte[] FrameBuffer;

        public bool Finished { get; private set; } = false;

        public enum OutputFormat {
            RGB24,
            RGBA64
        }

        public OutputFormat Format { get; private set; } = OutputFormat.RGB24;
        string _ffmpegPixelFormat => Format switch {
            OutputFormat.RGB24 => "rgb24",
            OutputFormat.RGBA64 => "rgba64le",
            _ => throw new NotSupportedException($"Output format {Format} is not supported.")
        };

        int _pixelSize => Format switch {
            OutputFormat.RGB24 => 3,
            OutputFormat.RGBA64 => 8,
            _ => throw new NotSupportedException($"Output format {Format} is not supported.")
        };

        public TextureFormat TextureFormat => Format switch {
            OutputFormat.RGB24 => TextureFormat.RGB24,
            OutputFormat.RGBA64 => TextureFormat.RGBA64,
            _ => throw new NotSupportedException($"Output format {Format} is not supported.")
        };


        public FFMPEGImageReader(string inputPath, OutputFormat format, Action<string>? logger = null) {
            _inputPath = inputPath;
            _logger = logger;
            Format = format;
            
            _mediaInfo = FFProbe.GetMediaInfo(inputPath);
            _videoStreamInfo = _mediaInfo.GetFirstVideoStream();
            if (_videoStreamInfo == null) {
                throw new InvalidOperationException("No video stream found in the media file.");
            }

            var frameSize = _videoStreamInfo.Width * _videoStreamInfo.Height * _pixelSize;
            FrameBuffer = new byte[frameSize];
            
        }

        string CreateFrameSelectFilter(int startFrame, int frameStride) {
            return $"select='gte(n\\,{startFrame})*not(mod(n\\,{frameStride}))'";
        }

        string CreateFrameSelectFilter(IList<Range> frameRanges) {
            if (frameRanges == null || frameRanges.Count == 0) {
                throw new ArgumentException("Frame ranges cannot be null or empty.", nameof(frameRanges));
            }

            var conditions = new List<string>();
            foreach (var range in frameRanges) {
                int startIndex = range.Start.IsFromEnd ? _videoStreamInfo.NumberOfFrames - range.Start.Value - 1 : range.Start.Value;
                int endIndex = range.End.IsFromEnd ? _videoStreamInfo.NumberOfFrames - range.End.Value - 1 : range.End.Value;
                conditions.Add($"between(n\\,{startIndex}\\,{endIndex})");
            }
            var selectExpr = string.Join("+", conditions);
            return $"select='{selectExpr}'";
        }

        void RunWithFilter(string filter) {
            // '-vsync 0' makes FFmpeg output only selected frames, no duplicates.
            var parameters = $"-vf {filter} -vsync passthrough -f rawvideo -pix_fmt {_ffmpegPixelFormat} -";

            RunInternal(parameters);
        }

        void RunInternal(string parameters) {
            var arguments = $"-i \"{_inputPath}\" {parameters}";
            ffmpegProcess = FFMPEG.Run(arguments, _logger);
        }

        public void Run(int startFrame = 0, int frameStride = 1) {
            if (startFrame < 0 || frameStride < 1) {
                throw new ArgumentOutOfRangeException("startFrame must be >= 0 and frameStride must be >= 1.");
            }
            string filter = "vflip";
            if (startFrame > 0 || frameStride > 1) {
                var select = CreateFrameSelectFilter(startFrame, frameStride);
                filter = $"{select},{filter}";
            }
            RunWithFilter(filter);
        }
        public void Run(params Range[] frameRanges) => Run(frameRanges as IList<Range>);
        public void Run(IList<Range> frameRanges) {
            string filter = "vflip";
            var select = CreateFrameSelectFilter(frameRanges);
            filter = $"{select},{filter}";            
            RunWithFilter(filter);
        }

        void ThrowIfCannotRun() {
            if (ffmpegProcess != null) {
                throw new InvalidOperationException("FFMPEG process is already running. Call Dispose() before starting a new run.");
            }
        }

        public void Terminate(int waitForExitForMilliseconds = 1000) {
            Finished = true;
            if (readFrameEnumerator != null) {
                readFrameEnumerator.Dispose();
                readFrameEnumerator = null;
            }

            if (ffmpegProcess != null) {
                if (!ffmpegProcess.HasExited) {
                    ffmpegProcess.CancelErrorRead();
                    ffmpegProcess.WaitForExit(waitForExitForMilliseconds);
                    if (!ffmpegProcess.HasExited) {
                        ffmpegProcess.Kill();
                        UnityEngine.Debug.LogWarning("FFMPEG process was killed due to timeout.");
                    }
                }
                ffmpegProcess.Dispose();
                ffmpegProcess = null;
            }

            
            
        }

        ~FFMPEGImageReader() {
            Dispose();
        }
        public void Dispose() {
            Terminate(0);
        }

        public bool ReadFrame(int milliseconds = int.MaxValue) {
            if (ffmpegProcess == null) {
                throw new InvalidOperationException("FFMPEG process is not running. Call Run() before reading frames.");
            }

            if (Finished) return false;

            if (readFrameEnumerator == null)
                readFrameEnumerator = ReadFrame(ffmpegProcess.StandardOutput.BaseStream).GetEnumerator();


            Stopwatch stopwatch = Stopwatch.StartNew();

            do {

                readFrameEnumerator.MoveNext();
                var status = readFrameEnumerator.Current;

                if (status == ReadFrameStatus.EndOfStream) {
                    Dispose();
                    return false;
                }

                if (status == ReadFrameStatus.Done) {
                    readFrameEnumerator.Dispose();
                    readFrameEnumerator = null;
                    return true;

                    /*if (copyData) {
                        var copy = FrameBuffer!;
                        frameBuffer = new byte[copy.Length];
                        return new DPX(copy);
                    }

                    return new DPX(FrameBuffer);*/
                }
            }
            while (stopwatch.ElapsedMilliseconds < milliseconds);

            return false; 
        }



        enum ReadFrameStatus { 
            InProgress,
            Done,
            EndOfStream
        }

        private IEnumerable<ReadFrameStatus> ReadFrame(Stream stream) {
            int offset = 0;
            int bytesToRead = FrameBuffer.Length;
            while (bytesToRead > 0) {
                int bytesRead = stream.Read(FrameBuffer, offset, bytesToRead);
                if (bytesRead == 0) {
                    yield return ReadFrameStatus.EndOfStream;
                    yield break;
                }

                offset += bytesRead;
                bytesToRead -= bytesRead;
                yield return ReadFrameStatus.InProgress;
            }
            yield return ReadFrameStatus.Done;
        }


        


    }



}
