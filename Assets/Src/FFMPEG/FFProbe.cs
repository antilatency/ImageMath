using Newtonsoft.Json;
using System.IO;
using System.Linq;

namespace ImageMath {
    public class MediaInfo {
        [JsonProperty("streams")]
        public StreamInfo[] Streams { get; set; }

        [JsonProperty("format")]
        public FormatInfo Format { get; set; }

        public StreamInfo GetFirstVideoStream() => Streams.First(x => x.CodecType == "video");
    }

    public class StreamInfo {
        [JsonProperty("index")]
        public int Index { get; set; }

        [JsonProperty("codec_name")]
        public string CodecName { get; set; }

        [JsonProperty("codec_type")]
        public string CodecType { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }

        [JsonProperty("duration")]
        public float Duration { get; set; }

        [JsonProperty("bit_rate")]
        public long BitRate { get; set; }

        [JsonProperty("nb_frames")]
        public int NumberOfFrames { get; set; }

    }


    public class FormatInfo {
        [JsonProperty("filename")]
        public string Filename { get; set; }

        [JsonProperty("nb_streams")]
        public int NumberOfStreams { get; set; }

        [JsonProperty("nb_programs")]
        public int NumberOfPrograms { get; set; }

        [JsonProperty("format_name")]
        public string FormatName { get; set; }

        [JsonProperty("format_long_name")]
        public string FormatLongName { get; set; }

        [JsonProperty("start_time")]
        public string StartTime { get; set; }

        [JsonProperty("duration")]
        public string Duration { get; set; }

        [JsonProperty("size")]
        public string Size { get; set; }

        [JsonProperty("bit_rate")]
        public string BitRate { get; set; }

        [JsonProperty("probe_score")]
        public int ProbeScore { get; set; }
    }

    public static class FFProbe {
        public static MediaInfo GetMediaInfo(string mediaFilePath) {
            var json = GetJson(mediaFilePath);
            MediaInfo mediaInfo = JsonConvert.DeserializeObject<MediaInfo>(json);
            return mediaInfo;
        }
        public static string GetJson(string mediaFilePath) {

            string ffmpegPath = Path.Combine(UnityEngine.Application.streamingAssetsPath, "ffmpeg", "bin", "ffprobe.exe");

            string arguments = $"-v error -print_format json -show_format -show_streams \"{mediaFilePath}\"";

            var process = new System.Diagnostics.Process();
            process.StartInfo.FileName = ffmpegPath;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            // process.OutputDataReceived += (sender, e) => Debug.Log(e.Data);
            //process.ErrorDataReceived += (sender, e) => Debug.LogError(e.Data);

            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            process.WaitForExit();



            if (process.ExitCode == 0) {

                return output;
            }
            else {
                throw new System.Exception(error);
            }
        }

    }
}