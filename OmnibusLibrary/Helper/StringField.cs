using System;
using System.Linq;

namespace Omnibus.Library.Helper {
	public class StringField {
		public StringField(UInt32 maxLength) {
			MaxLength = maxLength;
			Buffer = new byte[maxLength];
		}
		public StringField(int maxLength) : this((UInt32)maxLength) { }

		public static implicit operator string(StringField source) => source.AsString;
		protected string AsString {
			get => new(Buffer.TakeWhile(b => b != NullByte).Select(b => (char)b).ToArray());
		}
		public StringField Set(byte[] bytes) {
			Buffer = new byte[MaxLength];
			Array.Copy(bytes, Buffer, Math.Min(MaxLength, bytes.Length));
			return this;
		}
		public StringField Set() => Set(EmptyBuffer);

		protected string Value { get; init; }
		protected uint MaxLength { get; init; }
		protected byte[] Buffer { get; private set; }

		protected static readonly byte[] EmptyBuffer = new byte[] { };
		protected static byte NullByte = 0;
	}
}
