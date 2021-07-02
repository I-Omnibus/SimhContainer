using System;
using System.Linq;

namespace Omnibus.Library.Data {
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
	public class MapAttribute : Attribute {
		public MapAttribute(UInt16 Offset, UInt16 Length, Type Encoding)
			=> (this.Offset, this.Length, this.Encoding) = (Offset, Length, Encoding);

		internal UInt16 Offset { get; init; }
		internal UInt16 Length { get; init; }
		internal Type Encoding { get; init; }

		public record FieldData(string Name, int Offset, int Length, Type Encoding);
		public static FieldData GetFieldData(Enum mappedField) {

			var name = mappedField.ToString();
			var attribute = (MapAttribute)mappedField.GetType()
				.GetMember(name)
				.FirstOrDefault()
				.GetCustomAttributes(typeof(MapAttribute), false)
				.FirstOrDefault()
				;

			var offset = (attribute?.Offset).HasValue ?
				attribute.Offset
				:
				UInt16.MinValue
				;
			var length = (attribute?.Length).HasValue ?
				attribute.Length
				:
				UInt16.MinValue
				;
			var encoding = (attribute?.Encoding is not null) ?
				attribute.Encoding
				:
				typeof(object)
				;

			return new(name, offset, length, encoding);

		}
	}
}