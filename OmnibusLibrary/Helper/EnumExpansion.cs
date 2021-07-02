using System;
using System.Collections.Generic;
using System.Linq;

namespace Omnibus.Library.Helper {
	public static class EnumExpansion<TEnum> where TEnum : Enum {
		public static IEnumerable<TEnum> All {
			get => Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
		}
	}
}
