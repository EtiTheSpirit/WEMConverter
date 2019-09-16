using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WEMCompiler.FFmpegHook;
using WEMCompiler.WWWem.DataChunks;

namespace WEMCompiler {

	/// <summary>
	/// Nasty, Y U K K I, lazy, But also very short version of a WAV &lt;=&gt; WEM converter.<para/>
	/// At all costs do NOT use this.
	/// </summary>
	/// 
	[Obsolete("This method is horrible and will break for most WAV files. It was designed as a proof of concept. Use WEMFile.cs instead.", true)]
	public class WEMConverter {

		/// <summary>
		/// Converts the specified byte input stream, which should be a WAV file, into a WEM file.
		/// </summary>
		/// <param name="wav">The WAV file to convert to WEM</param>
		/// <returns></returns>
		public static byte[] ConvertWAVDataToWEMData(byte[] wav) {
			byte[] wemOut = new byte[wav.Length + 20];
			byte[] startInfo = wav.Take(36).ToArray();
			byte[] dataOnward = wav.Skip(36).ToArray();

			// New file size.
			byte[] newSize = BitConverter.GetBytes(BitConverter.ToUInt16(startInfo, 4) + 20);
			startInfo[4] = newSize[0];
			startInfo[5] = newSize[1];

			// New format chunk size (24 in wem, 16 in wav)
			startInfo[16] = 0x18;
			startInfo[17] = 0x00;

			// New format version flag
			startInfo[20] = 0xFE;
			startInfo[21] = 0xFF;

			startInfo.CopyTo(wemOut, 0);
			WEMFormatChunk.JunkData.CopyTo(wemOut, 36);
			dataOnward.CopyTo(wemOut, 56);
			return wemOut;
		}

		/// <summary>
		/// Converts the specified byte input stream, which should be a WEM file, into a WAV file.
		/// </summary>
		/// <param name="wem">The WEM file to convert to WAV</param>
		/// <returns></returns>
		public static byte[] ConvertWEMDataToWAVData(byte[] wem) {
			byte[] wavOut = new byte[wem.Length - 20];
			byte[] startInfo = wem.Take(36).ToArray();
			byte[] dataOnward = wem.Skip(56).ToArray(); // hey this is 56 instead of 36 now ok

			// New file size.
			byte[] newSize = BitConverter.GetBytes(BitConverter.ToUInt16(startInfo, 4) - 20);
			startInfo[4] = newSize[0];
			startInfo[5] = newSize[1];

			// New format chunk size (24 in wem, 16 in wav)
			startInfo[16] = 0x10;
			startInfo[17] = 0x00;

			// New format version flag
			startInfo[20] = 0x01;
			startInfo[21] = 0x00;

			startInfo.CopyTo(wavOut, 0);
			dataOnward.CopyTo(wavOut, 36);
			return wavOut;
		}

	}
}
