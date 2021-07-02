using System;
using System.Collections.Generic;
using System.Linq;

namespace Omnibus.Library.HashAlgorithm {

	public sealed class CRC32 : System.Security.Cryptography.HashAlgorithm {
		public const UInt32 DefaultPolynomial = 0b_1110_1101_1011_1000_1000_0011_0010_0000;
		public const UInt32 DefaultSeed = 0b_1111_1111_1111_1111_1111_1111_1111_1111;

		static UInt32[] defaultTable;

		UInt32 Seed { get; init; }
		UInt32[] Table { get; init; }
		UInt32 CurrentHash { get; set; }

		public CRC32() : this(DefaultPolynomial, DefaultSeed) { }

		public CRC32(UInt32 polynomial, UInt32 seed) {
			Table = InitializeTable(polynomial);
			Seed = CurrentHash = seed;
		}

		public override void Initialize() => CurrentHash = Seed;

		protected override void HashCore(byte[] array, int ibStart, int cbSize)
			=> CurrentHash = CalculateHash(Table, CurrentHash, array, ibStart, cbSize);

		protected override byte[] HashFinal()
			=> HashValue = UInt32ToBigEndianBytes(~CurrentHash);

		public override int HashSize { get => 32; }

		public static UInt32 Compute(byte[] buffer)
			=> Compute(DefaultSeed, buffer);

		public static UInt32 Compute(UInt32 seed, byte[] buffer)
			=> Compute(DefaultPolynomial, seed, buffer);

		public static UInt32 Compute(UInt32 polynomial, UInt32 seed, byte[] buffer)
			=> ~CalculateHash(InitializeTable(polynomial), seed, buffer, 0, buffer.Length);

		static UInt32[] InitializeTable(UInt32 polynomial) {
			if (polynomial == DefaultPolynomial && defaultTable != null)
				return defaultTable;

			var table = new UInt32[256];
			for (var i = 0; i < table.Length; i++) {
				var entry = (UInt32)i;
				for (var j = 0; j < 8; j++) {
					if ((entry & 1) == 1) {
						entry = (entry >> 1) ^ polynomial;
					} else {
						entry >>= 1;
					}
				}
				table[i] = entry;
			}

			if (polynomial == DefaultPolynomial) defaultTable = table;

			return table;
		}

		static UInt32 CalculateHash(UInt32[] table, UInt32 seed, IList<byte> buffer, int start, int size) {
			var hash = seed;
			for (var i = start; i < start + size; i++)
				hash = (hash >> 8) ^ table[buffer[i] ^ hash & 0xff];
			return hash;
		}

		static byte[] UInt32ToBigEndianBytes(UInt32 value)
			=> BitConverter.GetBytes(value).Reverse().ToArray();

	}
}