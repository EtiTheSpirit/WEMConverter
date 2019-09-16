using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WEMCompiler.WWWem.DataChunks {

	/// <summary>
	/// Represents the WEM's format chunk. This is identical to Microsoft's WAVE format, with the exception of <see cref="JunkData"/> being added to WEM files for an unknown reason.
	/// </summary>
	public class WEMFormatChunk {

		/// <summary>
		/// The ID of this chunk.
		/// </summary>
		public static readonly string ID = "fmt ";

		/// <summary>
		/// The size of this chunk in bytes, used as part of the total value for the <seealso cref="WEMHeader.FileSize"/> property. This value is 16 in WAV files.
		/// </summary>
		public int Size {
			get {
				return 24;
			}
		}

		/// <summary>
		/// The tag for this format. Unlike WAV files, this is 0xFE 0xFF instead of 0x01 0x00.
		/// </summary>
		public ushort FormatTag = 0xFFFE; // Interestingly, this is just WAV's format tag inverted (wav is 01 00, this is FE FF)

		/// <summary>
		/// The amount of channels in this file.
		/// </summary>
		public ushort Channels = 1;

		/// <summary>
		/// The sample rate of this file.
		/// </summary>
		public uint SampleRate = 48000;

		/// <summary>
		/// The average number of bytes per second, used to attempt calculation of RAM allocation.
		/// </summary>
		public uint AverageBytesPerSecond {
			get {
				return SampleRate * SampleFrameSize;
			}
		}

		/// <summary>
		/// The size of a sample.
		/// </summary>
		public ushort SampleFrameSize {
			get {
				return (ushort)(Channels * (BitsPerSample / 8));
			}
		}

		/// <summary>
		/// The number of bits in a sample.
		/// </summary>
		public ushort BitsPerSample = 16;

		/// <summary>
		/// The JUNK data stored within the WEM file. This does not contribute to the <see cref="Size"/> property, as it is technically its own chunk. <see cref="Size"/> is 8 bytes larger than in WAV due to the first 8 bytes of this array.
		/// </summary>
		// This is identical in every WEM file.
		public static readonly byte[] JunkData = new byte[] { 0x06, 0x00, 0x00, 0x00, 0x01, 0x41, 0x00, 0x00, 0x4A, 0x55, 0x4E, 0x4B, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

		/// <summary>
		/// Writes this format chunk to a binary writer.
		/// </summary>
		/// <param name="writer">The <see cref="BinaryWriter"/> to put the data into.</param>
		public void WriteToStream(BinaryWriter writer) {
			writer.Write(ID.ToCharArray());
			writer.Write(Size);
			writer.Write(FormatTag);
			writer.Write(Channels);
			writer.Write(SampleRate);
			writer.Write(AverageBytesPerSecond);
			writer.Write(SampleFrameSize);
			writer.Write(BitsPerSample);
			writer.Write(JunkData);
		}

		/// <summary>
		/// Creates a new WEMFormatChunk from a binary reader.<para/>
		/// ENSURE YOU'RE CALLING IN THE RIGHT ORDER: WEMHeader =&gt; WEMFormatChunk =&gt; WEMDataChunk.
		/// </summary>
		/// <param name="reader">The <see cref="BinaryReader"/> to take the data from.</param>
		/// <returns></returns>
		public static WEMFormatChunk CreateFromStream(BinaryReader reader) {
			WEMFormatChunk fmtChunk = new WEMFormatChunk();
			reader.ReadUInt32(); // Skip ID.
			reader.ReadUInt32(); // Skip size.
								 //fmtChunk.FormatTag = reader.ReadUInt16();
			reader.ReadUInt16(); // Skip format tag. This is so that casting works fine.
			fmtChunk.Channels = reader.ReadUInt16();
			fmtChunk.SampleRate = reader.ReadUInt32();
			reader.ReadUInt32(); // Skip avg bytes / sec
			reader.ReadUInt16(); // Skip frame size
			fmtChunk.BitsPerSample = reader.ReadUInt16();
			reader.Read(new byte[20], 0, 20); // skip junk data
			return fmtChunk;
		}

		public static implicit operator WAVFormatChunk(WEMFormatChunk fmt) {
			WAVFormatChunk chunk = new WAVFormatChunk {
				// Don't mess with the format tag. That needs to stay as its default.
				Channels = fmt.Channels,
				SampleRate = fmt.SampleRate,
				BitsPerSample = fmt.BitsPerSample
			};
			return chunk;
		}
	}

	public class WAVFormatChunk {
		/// <summary>
		/// The ID of this chunk.
		/// </summary>
		public static readonly string ID = "fmt ";

		/// <summary>
		/// The size of this chunk in bytes, used as part of the total value for the <seealso cref="WEMHeader.FileSize"/> property. This value is 24 in WEM files.
		/// </summary>
		public int Size {
			get {
				return 16;
			}
		}

		/// <summary>
		/// The tag for this format.
		/// </summary>
		public ushort FormatTag = 0x0001;

		/// <summary>
		/// The amount of channels in this file.
		/// </summary>
		public ushort Channels = 1;

		/// <summary>
		/// The sample rate of this file.
		/// </summary>
		public uint SampleRate = 48000;

		/// <summary>
		/// The average number of bytes per second, used to attempt calculation of RAM allocation.
		/// </summary>
		public uint AverageBytesPerSecond {
			get {
				return SampleRate * SampleFrameSize;
			}
		}

		/// <summary>
		/// The size of a sample.
		/// </summary>
		public ushort SampleFrameSize {
			get {
				return (ushort)(Channels * (BitsPerSample / 8));
			}
		}

		/// <summary>
		/// The number of bits in a sample.
		/// </summary>
		public ushort BitsPerSample = 16;

		/// <summary>
		/// Writes this format chunk to a binary writer.
		/// </summary>
		/// <param name="writer">The <see cref="BinaryWriter"/> to put the data into.</param>
		public void WriteToStream(BinaryWriter writer) {
			writer.Write(ID.ToCharArray());
			writer.Write(Size);
			writer.Write(FormatTag);
			writer.Write(Channels);
			writer.Write(SampleRate);
			writer.Write(AverageBytesPerSecond);
			writer.Write(SampleFrameSize);
			writer.Write(BitsPerSample);
		}

		/// <summary>
		/// Creates a new WAVFormatChunk from a binary reader.<para/>
		/// ENSURE YOU'RE CALLING IN THE RIGHT ORDER: WAVHeader =&gt; WAVFormatChunk =&gt; WAVDataChunk.
		/// </summary>
		/// <param name="reader">The <see cref="BinaryReader"/> to take the data from.</param>
		/// <returns></returns>
		public static WAVFormatChunk CreateFromStream(BinaryReader reader) {
			WAVFormatChunk fmtChunk = new WAVFormatChunk();
			reader.ReadUInt32(); // Skip ID.
			reader.ReadInt32(); // Skip size.
								 //fmtChunk.FormatTag = reader.ReadUInt16();
			reader.ReadUInt16(); // Skip format tag. This is so that casting works fine.
			fmtChunk.Channels = reader.ReadUInt16();
			fmtChunk.SampleRate = reader.ReadUInt32();
			reader.ReadUInt32(); // Skip avg bytes / sec
			reader.ReadUInt16(); // Skip frame size
			fmtChunk.BitsPerSample = reader.ReadUInt16();
			//reader.Read(new byte[20], 0, 20); // skip junk data
			return fmtChunk;
		}

		public static implicit operator WEMFormatChunk(WAVFormatChunk fmt) {
			WEMFormatChunk chunk = new WEMFormatChunk {
				// Don't mess with the format tag. That needs to stay as its default.
				Channels = fmt.Channels,
				SampleRate = fmt.SampleRate,
				BitsPerSample = fmt.BitsPerSample
			};
			return chunk;
		}
	}
}
