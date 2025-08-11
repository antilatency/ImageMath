using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;
#nullable enable
namespace ImageMath {

    public static class FFMPEG {
        public static string FFMpegDirectory => Path.Combine(UnityEngine.Application.streamingAssetsPath, "ffmpeg", "bin");
        public static string FFMpegPath => Path.Combine(FFMpegDirectory, "ffmpeg.exe");

        public static string[] SupportedFormats = new[]{
        "mp4",
        "mov",
       /* "avi",
        "mkv",
        "wmv",
        "mpeg",
        "mpg",
        "mpeg4",
        "mts",
        "m2ts",
        "ogv"*/
    };

        public static Process Run(
            string inputPath,
            string? parameters = null,
            Action<string>? logger = null) {
            return Run($"-i \"{inputPath}\" {parameters}", logger);
        }

        public static Process Run(string arguments, Action<string>? logger = null) {
            var process = new Process();
            process.StartInfo.FileName = FFMpegPath;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            if (logger != null) {
                process.ErrorDataReceived += (sender, e) => logger(e.Data);
            }

            process.Start();
            process.PriorityClass = ProcessPriorityClass.Idle;
            process.BeginErrorReadLine();

            return process;
        }


    }

}