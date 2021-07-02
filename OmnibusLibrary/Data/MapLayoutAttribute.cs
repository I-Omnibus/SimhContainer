using System;
using System.Linq;

namespace Omnibus.Library.Data {
	[AttributeUsage(AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
	public class MapLayoutAttribute : Attribute {
		public MapLayoutAttribute(UInt32 Size) => this.Size = Size;

		internal UInt32 Size { get; init; }

		public static UInt32? GetSize<TEnum>() where TEnum : Enum 
			=> ((MapLayoutAttribute)typeof(TEnum)
				.GetCustomAttributes(typeof(MapLayoutAttribute), inherit: false)
				.FirstOrDefault())?.Size;

	}
}