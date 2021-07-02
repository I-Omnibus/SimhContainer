using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Omnibus.Library.Helper;

namespace Omnibus.Library.Data {
	public abstract class Mapping<TEnum> where TEnum : Enum {
		public Mapping() {
			var sizeAttribute = MapLayoutAttribute.GetSize<TEnum>();
			if (false == sizeAttribute.HasValue)
				throw new ArgumentException($"{ThisTypeName.Value} requires enum '{typeof(TEnum).Name}' to have a Size attribute - [{nameof(MapLayoutAttribute)}(Size: value)]");

			Size = sizeAttribute.Value;
			Buffer = new byte[Size];
		}
		public UInt32 Size { get; init; }
		public byte[] Buffer { get; init; }
		public TResult GetField<TResult>(TEnum field)
			=> Type.GetTypeCode(typeof(TResult)) switch {
				TypeCode.Byte => Cast<TResult>(GetRawBytes(field).First()),
				TypeCode.UInt16 => Cast<TResult>(BitConverter.ToUInt16(GetRawBytes(field, littleEndian: true))),
				TypeCode.UInt32 => Cast<TResult>(BitConverter.ToUInt32(GetRawBytes(field, littleEndian: true))),
				TypeCode.UInt64 => Cast<TResult>(BitConverter.ToUInt64(GetRawBytes(field, littleEndian: true))),
				TypeCode.String => Cast<TResult>(GetString(field)),
				_ => throw new ArgumentException($"Unsupported Field type - '{typeof(TResult)}'.")
			};
		public void SetField<T>(TEnum field, T value = default) {
			var fieldData = MapAttribute.GetFieldData(field);
			switch (Type.GetTypeCode(typeof(T))) {
				case TypeCode.Byte:
				Buffer[fieldData.Offset] = Cast<byte>(value);
				break;
				case TypeCode.UInt16:
				SetRawBytes(field,BitConverter.GetBytes(Cast<UInt16>(value)),littleEndian:true);
				break;
				case TypeCode.UInt32:
				SetRawBytes(field, BitConverter.GetBytes(Cast<UInt32>(value)), littleEndian: true);
				break;
				case TypeCode.UInt64:
				SetRawBytes(field, BitConverter.GetBytes(Cast<UInt64>(value)), littleEndian: true);
				break;
				case TypeCode.String:
				var buffer = new byte[fieldData.Length];
				var bytes = Encoding.UTF8.GetBytes(Cast<string>(value));
				Array.Copy(bytes, 0, buffer, 0, Math.Min(bytes.Length, fieldData.Length));
				Array.Copy(buffer, 0, Buffer, fieldData.Offset, fieldData.Length);
				break;
				default:
				throw new ArgumentException($"Unsupported Field type - '{typeof(T)}'.");
			}
		}
		private static TResult Cast<TResult>(object dunno)
			=> new object[] { dunno }.Cast<TResult>().FirstOrDefault();
		private string GetString(TEnum field) {
			var buffer = GetRawBytes(field);
			var stringField = new StringField(buffer.Length);
			stringField.Set(buffer);
			return stringField;
		}
		protected byte[] GetRawBytes(TEnum field, bool littleEndian = false) {
			var fieldData = MapAttribute.GetFieldData(field);
			var bytes = Bytes(fieldData.Offset, fieldData.Length);
			return littleEndian ?
				bytes.Reverse().ToArray()
				:
				bytes.ToArray()
				;
		}
		protected void SetRawBytes(TEnum field, byte[] bytes, bool littleEndian = false) {
			var fieldData = MapAttribute.GetFieldData(field);
			if (littleEndian) {
				Array.Copy(bytes.Reverse().ToArray(), 0, Buffer, fieldData.Offset, fieldData.Length);
			} else {
				Array.Copy(bytes, 0, Buffer, fieldData.Offset, fieldData.Length);
			}
		}
		private IEnumerable<byte> Bytes(int offset, int length)
			=> Buffer.Skip(offset).Take(length);

		protected static Lazy<string> ThisTypeName {
			get => new(() => typeof(Mapping<TEnum>).Name.Split('`').First());
		}

		public override string ToString() {
			var result = string.Empty;
			var fieldDataList = new List<MapAttribute.FieldData>();
			foreach (var field in EnumExpansion<TEnum>.All) {
				fieldDataList.Add(
					new(
						MapAttribute.GetFieldData(field).Name,
						MapAttribute.GetFieldData(field).Offset,
						MapAttribute.GetFieldData(field).Length,
						MapAttribute.GetFieldData(field).Encoding
					)
				);
			}

			foreach (var fieldData in fieldDataList) result += $"{fieldData}\n";

			return result;
		}
	}
}
