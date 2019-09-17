using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WEMCompiler.WWWem.DataChunks {

	/// <summary>
	/// A WEM file header. This is identical to that of a WAV file.
	/// </summary>
	public class WEMHeader {
		/// <summary>
		/// The ID of this data block.
		/// </summary>
		public static readonly string ID = "RIFF";

		/// <summary>
		/// The size of this chunk in bytes.
		/// </summary>
		public int Size { get; } = 4;

		/// <summary>
		/// The total size of this file, minus RIFF and the integer space used to take this value. (File Size - 8 bytes)
		/// </summary>
		public int FileSize {
			get {
				return Size + DataChunk.Size + DataChunk.FormatChunk.Size;
			}
		}

		/// <summary>
		/// The type of RIFF this is. This never changes.
		/// </summary>
		public string RIFFType = "WAVE";

		/// <summary>
		/// A reference to the WEMDataChunk.
		/// </summary>
		public WEMDataChunk DataChunk { get; set; }

		/// <summary>
		/// Construct a new WEMHeader.
		/// </summary>
		/// <param name="dataChunk"></param>
		public WEMHeader(WEMDataChunk dataChunk) {
			DataChunk = dataChunk;
		}

		internal WEMHeader() { }

		/// <summary>
		/// Writes this header chunk to a binary writer.
		/// </summary>
		/// <param name="writer">The <see cref="BinaryWriter"/> to put the data into.</param>
		public void WriteToStream(BinaryWriter writer) {
			writer.Write(ID.ToCharArray());
			writer.Write(FileSize);
			writer.Write(RIFFType.ToCharArray());
		}

		/// <summary>
		/// Creates a new header chunk from a binary reader.<para/>
		/// ENSURE YOU'RE CALLING IN THE RIGHT ORDER: WEMHeader =&gt; WEMFormatChunk =&gt; WEMDataChunk.
		/// </summary>
		/// <param name="reader">The <see cref="BinaryReader"/> to take the data from.</param>
		/// <returns></returns>
		public static WEMHeader CreateFromStream(BinaryReader reader) {
			reader.ReadUInt32();
			reader.ReadInt32();
			reader.ReadUInt32();
			return new WEMHeader();
		}

		public static implicit operator WAVHeader(WEMHeader head) {
			return new WAVHeader();
		}

	}

	/// <summary>
	/// A WAV header. This is identical to <seealso cref="WEMHeader"/> as no data is different between the two, aside from file size being 20 bytes smaller.
	/// </summary>
	public class WAVHeader {
		/// <summary>
		/// The ID of this data block.
		/// </summary>
		public static readonly string ID = "RIFF";

		/// <summary>
		/// The size of this chunk in bytes.
		/// </summary>
		public int Size { get; } = 4;

		/// <summary>
		/// The total size of this file, minus RIFF and the integer space used to take this value. (File Size - 8 bytes)
		/// </summary>
		public int FileSize {
			get {
				return Size + DataChunk.Size + DataChunk.FormatChunk.Size;
			}
		}

		/// <summary>
		/// The type of RIFF this is. This never changes.
		/// </summary>
		public string RIFFType = "WAVE";

		/// <summary>
		/// A reference to the WEMDataChunk.
		/// </summary>
		public WAVDataChunk DataChunk { get; set; }

		/// <summary>
		/// Construct a new WEMHeader.
		/// </summary>
		/// <param name="dataChunk"></param>
		public WAVHeader(WAVDataChunk dataChunk) {
			DataChunk = dataChunk;
		}

		internal WAVHeader() { }

		/// <summary>
		/// Writes this header chunk to a binary writer.
		/// </summary>
		/// <param name="writer">The <see cref="BinaryWriter"/> to put the data into.</param>
		public void WriteToStream(BinaryWriter writer) {
			writer.Write(ID.ToCharArray());
			writer.Write(FileSize);
			writer.Write(RIFFType.ToCharArray());
		}

		/// <summary>
		/// Creates a new header chunk from a binary reader.<para/>
		/// ENSURE YOU'RE CALLING IN THE RIGHT ORDER: WAVHeader =&gt; WAVFormatChunk =&gt; WAVDataChunk.
		/// </summary>
		/// <param name="reader">The <see cref="BinaryReader"/> to take the data from.</param>
		/// <returns></returns>
		public static WAVHeader CreateFromStream(BinaryReader reader) {
			reader.ReadUInt32();
			reader.ReadInt32();
			reader.ReadUInt32();

			return new WAVHeader();
		}

		public static implicit operator WEMHeader(WAVHeader head) {
			return new WEMHeader();
		}
	}
}
