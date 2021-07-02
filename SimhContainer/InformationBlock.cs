using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using Omnibus.Library.HashAlgorithm;
using Omnibus.Library.Data;

namespace Omnibus.Simh.Container {
	public class InformationBlock : Mapping<FieldScope> {
		public InformationBlock(string filePath) {
			var fileInfo = new FileInfo(filePath);
			Exists = fileInfo.Exists;
			var length = fileInfo.Length;
			var lastBlockOffset = ((length / BytesPerBlock) - 1) * BytesPerBlock;
			File = MemoryMappedFile.CreateFromFile(filePath, FileMode.Open);
			FileAccessor = File.CreateViewAccessor(lastBlockOffset, BytesPerBlock);

			FileAccessor.ReadArray<byte>(0, Buffer, 0, Buffer.Length);
		}

		public UInt32 CalculatedChecksum {
			get {
				var result = UInt32.MinValue;
				var byteCount = Buffer.Length - MapAttribute.GetFieldData(FieldScope.Checksum).Length;
				var crc32 = new CRC32();
				var hash = crc32.ComputeHash(Buffer, 0, byteCount).Reverse().ToArray();
				result = BitConverter.ToUInt32(hash);

				return result;
			}
		}

		internal bool Exists { get; private set; }
		protected MemoryMappedFile File { get; init; }
		protected MemoryMappedViewAccessor FileAccessor { get; init; }

		protected const int BytesPerBlock = 512;
	}
}
