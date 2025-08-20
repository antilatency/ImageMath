using System.Collections.Generic;
using System.Diagnostics;
using System;
using System.IO;
using UnityEngine;
using System.Linq;
using System.Text;


#nullable enable
namespace ImageMath {

    /*public abstract class FFMPEGSelectFilter {
        public abstract string GetFilter();
    }
    
    public class FFMPEGSelectFilterRanges : FFMPEGSelectFilter {
        private readonly IList<Range> _ranges;

        public FFMPEGSelectFilterRanges(IList<Range> ranges) {
            _ranges = ranges ?? throw new ArgumentNullException(nameof(ranges));
        }

        public override string GetFilter() {
            if (_ranges.Count == 0) {
                throw new ArgumentException("Ranges cannot be empty.", nameof(_ranges));
            }

            var conditions = new List<string>();
            foreach (var range in _ranges) {
                int startIndex = range.Start.IsFromEnd ? -range.Start.Value - 1 : range.Start.Value;
                int endIndex = range.End.IsFromEnd ? -range.End.Value - 1 : range.End.Value;
                conditions.Add($"between(n\\,{startIndex}\\,{endIndex})");
            }
            var selectExpr = string.Join("+", conditions);
            return $"select='{selectExpr}'";
        }
    }

    public class FFMPEGSelectFilterStride : FFMPEGSelectFilter {
        private readonly int _startFrame;
        private readonly int _frameStride;

        public FFMPEGSelectFilterStride(int startFrame, int frameStride) {
            if (startFrame < 0) throw new ArgumentOutOfRangeException(nameof(startFrame), "Start frame must be >= 0.");
            if (frameStride < 1) throw new ArgumentOutOfRangeException(nameof(frameStride), "Frame stride must be >= 1.");

            _startFrame = startFrame;
            _frameStride = frameStride;
        }

        public override string GetFilter() {
            return $"select='gte(n\\,{_startFrame})*not(mod(n\\,{_frameStride}))'";
        }
    }

    public class FFMPEGSelectFilterSingleFrame : FFMPEGSelectFilter {
        private readonly int _frameIndex;

        public FFMPEGSelectFilterSingleFrame(int frameIndex) {
            if (frameIndex < 0) throw new ArgumentOutOfRangeException(nameof(frameIndex), "Frame index must be >= 0.");
            _frameIndex = frameIndex;
        }

        public override string GetFilter() {
            return $"select='eq(n\\,{_frameIndex})'";
        }
    }*/

    public class FFMPEGImageReader : IDisposable {
        string _inputPath;
        Action<string>? _logger;

        MediaInfo _mediaInfo;
        StreamInfo _videoStreamInfo;


        public int InputWidth => _videoStreamInfo.Width;
        public int InputHeight => _videoStreamInfo.Height;

        public int OutputWidth { get; private set; }
        public int OutputHeight { get; private set; }

        public float FPS => _videoStreamInfo.NumberOfFrames / _videoStreamInfo.Duration;

        public int NumberOfFrames => _videoStreamInfo.NumberOfFrames;

        public float Duration => _videoStreamInfo.Duration;

        Process? ffmpegProcess;
        IEnumerator<ReadFrameStatus>? readFrameEnumerator;

        public byte[] FrameBuffer { get; private set; }

        public bool Finished { get; private set; } = true;

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

            OutputWidth = _videoStreamInfo.Width;
            OutputHeight = _videoStreamInfo.Height;
        }

        public class RunParameters {
            public float InputSeek { get; set; } = 0;
            public bool AccurateInputSeek { get; set; } = false;
            public int OutputWidth { get; set; } = 0;
            public int OutputHeight { get; set; } = 0;
            public ScaleFlags ScaleFlags { get; set; } = ScaleFlags.Bilinear;
            public int NumberOfFrames { get; set; } = 0;
            public string Select { get; set; } = string.Empty;
        }

        public enum ScaleFlags {
            Bilinear,   // fast, decent quality
            Bicubic,    // smoother, medium quality
            Lanczos,    // best quality, slower
            Neighbor    // nearest neighbor, pixelated, fastest
        }

        public void Run(RunParameters? parameters = null) {
            if (parameters == null) {
                parameters = new RunParameters(); // default
            }
            var arguments = new StringBuilder();
            arguments.Append($"-nostats -hide_banner ");

            if (parameters.InputSeek > 0) {
                arguments.Append($"-ss {parameters.InputSeek} ");
                if (parameters.AccurateInputSeek) {
                    arguments.Append("-accurate_seek ");
                }
                else {
                    arguments.Append("-noaccurate_seek ");
                }
            }
            arguments.Append($"-i \"{_inputPath}\" ");
            //arguments.Append("-an -map 0:v"); // disable audio

            var filters = new List<string>();
            if (!string.IsNullOrEmpty(parameters.Select)) {
                filters.Add(parameters.Select);
            }
            if (parameters.OutputWidth > 0 && parameters.OutputHeight > 0) {
                filters.Add($"scale={parameters.OutputWidth}:{parameters.OutputHeight}:flags={parameters.ScaleFlags.ToString().ToLower()}");
                OutputWidth = parameters.OutputWidth;
                OutputHeight = parameters.OutputHeight;
            } else {
                OutputWidth = InputWidth;
                OutputHeight = InputHeight;
            }
            filters.Add("vflip"); // always flip vertically

            if (filters.Count > 0) {
                arguments.Append($"-vf \"{string.Join(",", filters)}\" ");
            }            

            if (parameters.NumberOfFrames > 0) {
                arguments.Append($"-frames:v {parameters.NumberOfFrames} ");
            }
            

            arguments.Append($"-fps_mode passthrough -f rawvideo -pix_fmt {_ffmpegPixelFormat} -");

            RunInternal(arguments.ToString());
        }

        /*public void Run(int width = 0, int height = 0, ScaleFlags scaleFlags = ScaleFlags.Bilinear, params string[] filters) {
            OutputWidth = width <= 0 ? InputWidth : width;
            OutputHeight = height <= 0 ? InputHeight : height;
            Run(filters.Append(CreateSizeFilter(OutputWidth, OutputHeight)).ToArray());
        }

        public void Run(params string[] filters) {
            // '-fps_mode passthrough' makes FFmpeg output only selected frames, no duplicates.
            var parameters = $"-nostats -hide_banner -vf {string.Join(",", filters.Append("vflip"))} -fps_mode passthrough -f rawvideo -pix_fmt {_ffmpegPixelFormat} -";

            RunInternal(parameters);
        }*/

        void RunInternal(string arguments) {
            Finished = false;
            var frameSize = OutputWidth * OutputHeight * _pixelSize;
            FrameBuffer = new byte[frameSize];
            ffmpegProcess = FFMPEG.Run(arguments, _logger);
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



        public string CreateSelectFilter(int startFrame, int frameStride = 0) {
            if (frameStride <= 0) {//single frame
                return $"select='eq(n\\,{startFrame})'";
            }
            return $"select='gte(n\\,{startFrame})*not(mod(n\\,{frameStride}))'";
        }

        public string CreateSelectFilter(params Range[] frameRanges) {
            if (frameRanges == null || frameRanges.Length == 0) {
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

        private string CreateSizeFilter(int width, int height, ScaleFlags scaleFlags = ScaleFlags.Bilinear) {
            if (width <= 0 || height <= 0) {
                throw new ArgumentOutOfRangeException("Width and height must be greater than 0.");
            }
            return $"scale={width}:{height}:flags={scaleFlags.ToString().ToLower()}";
        }
        


    }



}
