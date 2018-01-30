using System;
using System.Net;
using System.Text.RegularExpressions;

namespace Husky
{
	public static class StringTest
	{
		public const string EmailRegexPattern = @"^[0-9a-zA-Z][-0-9a-zA-Z.+_]+@[-0-9a-zA-Z.+_]+\.[a-zA-Z]{2,4}$";
		public const string MainlandMobileRegexPattern = @"^1[3578]\d{9}$";

		public static bool IsInt32(this string str) => int.TryParse(str, out var i);
		public static bool IsInt64(this string str) => long.TryParse(str, out var i);
		public static bool IsNumeric(this string str) => decimal.TryParse(str, out var d);
		public static bool IsBoolean(this string str) => bool.TryParse(str, out var b);
		public static bool IsDateTime(this string str) => DateTime.TryParse(str, out var dt);
		public static bool IsIPv4(this string str) => IPAddress.TryParse(str, out var ip);

		public static bool IsUrl(this string str) {
			return (str == null || str.Length < 6) ? false : Uri.IsWellFormedUriString(str, UriKind.Absolute);
		}
		public static bool IsVolumePath(this string path) {
			return (path == null || path.Length < 3) ? false : path.Contains(":");
		}
		public static bool IsEmail(this string str) {
			return (str == null || str.Length < 6) ? false : Regex.IsMatch(str, EmailRegexPattern);
		}
		public static bool IsCardNumber(this string str) {
			return (str == null || (str.Length != 16 && str.Length != 19)) ? false : Regex.IsMatch(str, @"^\d+$");
		}
		public static bool IsMainlandMobile(this string str) {
			return (str == null || str.Length != 11) ? false : Regex.IsMatch(str, MainlandMobileRegexPattern);
		}

		internal static bool IsMainlandSocialNumber(this string str) {
			return str.Length == 18 && Regex.IsMatch(str, @"^\d{18}$");
		}
		public static bool IsMainlandSocialNumber(this string str, Sex sex) {
			if ( str == null || str.Length != 18 ) return false;
			if ( sex == Sex.Male && (str[16] - '0') % 2 == 0 ) return false;
			if ( sex == Sex.Female && (str[16] - '0') % 2 == 1 ) return false;
			var times = new[] { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };
			var n = 0;
			for ( int i = 0; i < 17; n += (str[i] - '0') * times[i++] ) ;
			return new[] { '1', '0', 'X', '9', '8', '7', '6', '5', '4', '3', '2' }[n % 11] == str[17];
		}
	}
}