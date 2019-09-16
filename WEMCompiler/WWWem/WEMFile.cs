using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WEMCompiler.WWWem.DataChunks;

namespace WEMCompiler.WWWem {

	/// <summary>
	/// Represents a WEM file.
	/// </summary>
	public class WEMFile {

		/// <summary>
		/// The header of this WEM file.
		/// </summary>
		public WEMHeader Header { get; }

		/// <summary>
		/// The format information of this WEM file.
		/// </summary>
		public WEMFormatChunk Format { get; }

		/// <summary>
		/// The data of this WEM file.
		/// </summary>
		public WEMDataChunk Data { get; }

		/// <summary>
		/// Create a new WEM file from the specified path.
		/// </summary>
		/// <param name="path">The WEM file.</param>
		public WEMFile(string path) {
			using (FileStream inputStream = new FileStream(path, FileMode.Open)) {
				BinaryReader reader = new BinaryReader(inputStream);

				Header = WEMHeader.CreateFromStream(reader);
				Format = WEMFormatChunk.CreateFromStream(reader);
				Data = WEMDataChunk.CreateFromStream(reader, Format);
				Header.DataChunk = Data;

				reader.Dispose();
			}
		}

		/// <summary>
		/// Create a new WEM file from data. Used mainly in the conversion methods.
		/// </summary>
		/// <param name="header">The header</param>
		/// <param name="fmtChunk">The format info</param>
		/// <param name="data">The data</param>
		public WEMFile(WEMHeader header, WEMFormatChunk fmtChunk, WEMDataChunk data) {
			Header = header;
			Format = fmtChunk;
			Data = data;
			Header.DataChunk = Data;
			Data.FormatChunk = Format;
		}

		/// <summary>
		/// Convert this WEM file into a WAV file.
		/// </summary>
		/// <returns></returns>
		public WAVFile ConvertToWAV() {
			return new WAVFile(Header, Format, Data);
		}

		/// <summary>
		/// Save this WEM file. Returns FileInfo for the file that was created.
		/// </summary>
		/// <param name="path">The file to save under.</param>
		public FileInfo SaveToFile(string path) {
			using (FileStream outStream = new FileStream(path, FileMode.Create)) {
				BinaryWriter writer = new BinaryWriter(outStream);
				Header.WriteToStream(writer);
				Format.WriteToStream(writer);
				Data.WriteToStream(writer);
				writer.Flush();
				writer.Dispose();
			}
			return new FileInfo(path);
		}
	}

	public class WAVFile {
		/// <summary>
		/// The header of this WAV file.
		/// </summary>
		public WAVHeader Header { get; }

		/// <summary>
		/// The format information of this WAV file.
		/// </summary>
		public WAVFormatChunk Format { get; }

		/// <summary>
		/// The data of this WAV file.
		/// </summary>
		public WAVDataChunk Data { get; }

		/// <summary>
		/// Create a new WEM file from the specified path.
		/// </summary>
		/// <param name="path">The WEM file.</param>
		public WAVFile(string path) {
			using (FileStream inputStream = new FileStream(path, FileMode.Open)) {
				BinaryReader reader = new BinaryReader(inputStream);

				Header = WAVHeader.CreateFromStream(reader);
				Format = WAVFormatChunk.CreateFromStream(reader);
				Data = WAVDataChunk.CreateFromStream(reader, Format);
				Header.DataChunk = Data;

				reader.Dispose();
			}
		}

		/// <summary>
		/// Create a new WAV file from data. Used mainly in the conversion methods.
		/// </summary>
		/// <param name="header">The header</param>
		/// <param name="fmtChunk">The format info</param>
		/// <param name="data">The data</param>
		public WAVFile(WAVHeader header, WAVFormatChunk fmtChunk, WAVDataChunk data) {
			Header = header;
			Format = fmtChunk;
			Data = data;
			Header.DataChunk = Data;
			Data.FormatChunk = Format;
		}

		/// <summary>
		/// Convert this WAV file into a WEM file.
		/// </summary>
		/// <returns></returns>
		public WEMFile ConvertToWEM() {
			return new WEMFile(Header, Format, Data);
		}

		/// <summary>
		/// Save this WAV file. Returns FileInfo for the file that was created.
		/// </summary>
		/// <param name="path">The file to save under.</param>
		public FileInfo SaveToFile(string path) {
			using (FileStream outStream = new FileStream(path, FileMode.Create)) {
				BinaryWriter writer = new BinaryWriter(outStream);
				Header.WriteToStream(writer);
				Format.WriteToStream(writer);
				Data.WriteToStream(writer);
				writer.Flush();
				writer.Dispose();
			}
			return new FileInfo(path);
		}
	}
}
