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
		public static bool IsMainlandMobile(this string str) {
			return (str == null || str.Length != 11) ? false : Regex.IsMatch(str, MainlandMobileRegexPattern);
		}
		public static bool IsCardNumber(this string str) {
			return (str == null || (str.Length != 16 && str.Length != 19)) ? false : Regex.IsMatch(str, @"^\d+$");
		}
	}
}