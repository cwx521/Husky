using System;
using System.Linq;
using Newtonsoft.Json;

namespace Husky
{
	public static class StringCast
	{
		public static string NullAsEmpty(this string str) {
			return str ?? "";
		}

		public static string EmptyAsNull(this string str) {
			return string.IsNullOrEmpty(str) ? null : str;
		}

		public static string EmptyOrWhiteSpaceAsNull(this string str) {
			return string.IsNullOrWhiteSpace(str) ? null : str;
		}

		public static int AsInt(this string str, int defaultValue = 0) {
			return int.TryParse(str, out var i) ? i : defaultValue;
		}

		public static bool AsBool(this string str, bool defaultValue = false) {
			return bool.TryParse(str, out var b) ? b : defaultValue;
		}

		public static Guid AsGuid(this string str, Guid defaultValue = default(Guid)) {
			return Guid.TryParse(str, out var g) ? g : defaultValue;
		}

		public static TimeSpan AsTimeSpan(this string str, TimeSpan defaultValue = default(TimeSpan)) {
			return TimeSpan.TryParse(str, out var t) ? t : defaultValue;
		}

		public static T As<T>(this string str, T defaultValue = default(T)) {
			if ( str == null ) {
				return default(T);
			}
			if ( typeof(T) == typeof(Guid) ) {
				return (T)(object)str.AsGuid((Guid)(object)defaultValue);
			}
			if ( typeof(T) == typeof(TimeSpan) ) {
				return (T)(object)str.AsTimeSpan((TimeSpan)(object)defaultValue);
			}
			try {
				if ( typeof(IConvertible).IsAssignableFrom(typeof(T)) ) {
					return (T)Convert.ChangeType(str, typeof(T));
				}
				return JsonConvert.DeserializeObject<T>(str);
			}
			catch {
				return defaultValue;
			}
		}

		public static double HexToDouble(this string hex) {
			if ( string.IsNullOrEmpty(hex) ) {
				throw new ArgumentNullException(nameof(hex));
			}

			hex = hex.ToLower();
			if ( hex.StartsWith("0x") ) {
				hex = hex.Substring(2);
			}

			double dbl = 0;
			var charArray = hex.Reverse().ToArray();

			for ( int i = 0; i < charArray.Count(); i++ ) {
				var num = 0;
				var c = charArray[i];

				if ( c >= 'a' && c <= 'f' ) {
					num = 10 + (c - 'a');
				}
				else if ( c >= '0' && c <= '9' ) {
					num = (c - '0');
				}
				else {
					throw new FormatException($"{hex} is not a valid hex number string.");
				}
				dbl += num * Math.Pow(16, i);
			}
			return dbl;
		}

		public static string Mask(this string str) {
			if ( string.IsNullOrWhiteSpace(str) ) {
				return str;
			}
			if ( str.Length == 2 || str.Length == 3 ) {
				return $"{str[0]}{new string('*', str.Length - 1)}";
			}
			if ( str.IsMainlandMobile() ) {
				return $"{str.Substring(0, 3)}****{str.Substring(7)}";
			}
			if ( str.IsEmail() ) {
				var at = str.IndexOf('@');
				return $"{str.Substring(0, 1)}{new string('*', str.Length - 1 - at)}{str.Substring(at)}";
			}
			if ( str.IsCardNumber() ) {
				return $"{str.Substring(0, 4)}{new string('*', str.Length - 8)}{str.Substring(str.Length - 4)}";
			}
			if ( str.IsMainlandSocialNumber() ) {
				return $"{str.Substring(0, 6)}{new string('*', str.Length - 8)}{str.Substring(str.Length - 4)}";
			}
			return str;
		}
	}
}