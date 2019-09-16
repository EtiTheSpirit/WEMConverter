using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace WEMCompiler.FFmpegHook {
	public class FFmpegWrapper {

		public static byte[] GetAudioPCM(string fileName) {
			ProcessStartInfo ffmpegInfo = new ProcessStartInfo {
				FileName = "ffmpeg",
				Arguments = $"-i \"{fileName}\" -ac 2 -f s16le -ar 48000 pipe:1",
				UseShellExecute = false,
				RedirectStandardOutput = true,
				RedirectStandardError = true
			};
			Process ffmpeg = Process.Start(ffmpegInfo);
			Stream ffout = ffmpeg.StandardOutput.BaseStream;

			// Buffer ffmpeg's output into a memory stream to get a byte array.
			byte[] pcm;
			using (var ms = new MemoryStream()) {
				ffout.CopyTo(ms);
				ms.Position = 0;
				pcm = ms.ToArray();
			}
			return pcm;
		}

		/// <summary>
		/// Takes in the specified file and converts it to a wav file using ffmpeg. Returns FileInfo for the new file.
		/// </summary>
		/// <param name="fileName">The audio file, or video file to take audio from.</param>
		/// <param name="tempConversionDir">A reference to a temporary directory to store converted WAV files. If this is null, a default directory will be created.</param>
		/// <returns></returns>
		public static FileInfo ConvertToWaveFile(string fileName, DirectoryInfo tempConversionDir = null) {
			if (tempConversionDir == null) {
				tempConversionDir = Directory.CreateDirectory(@".\TEMP");
			}
			ProcessStartInfo ffmpegInfo = new ProcessStartInfo {
				FileName = "ffmpeg",
				Arguments = $"-i \"{fileName}\" -y -f wav " + tempConversionDir.FullName + @"\export.wav",
				UseShellExecute = false,
				RedirectStandardOutput = true,
				RedirectStandardError = true
			};
			Process ffmpeg = Process.Start(ffmpegInfo);
			ffmpeg.WaitForExit();
			return new FileInfo(@".\TEMP\export.wav");
		}

		/// <summary>
		/// Using ffmpeg, this takes in a specific file, converts it to the WAV format, and returns the WAV file as a byte array.
		/// </summary>
		/// <param name="fileName">The audio file, or video file to take audio from.</param>
		/// <returns></returns>
		public static byte[] GetFileAsWAVData(string fileName) {
			byte[] output;
			using (FileStream fs = new FileStream(ConvertToWaveFile(fileName).FullName, FileMode.Open)) {
				using (MemoryStream ms = new MemoryStream()) {
					fs.CopyTo(ms);
					output = ms.ToArray();
				}
			}
			return output;
		}

	}
}
