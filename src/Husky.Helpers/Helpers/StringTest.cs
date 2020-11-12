using System;
using System.Net;
using System.Text.RegularExpressions;

namespace Husky
{
	public static class StringTest
	{
		public const string EmailRegexPattern = @"^[0-9a-zA-Z][-0-9a-zA-Z.+_]+@[-0-9a-zA-Z.+_]+\.[a-zA-Z]{2,4}$";
		public const string MainlandMobileRegexPattern = @"^1[3456789]\d{9}$";
		public const string MainlandSocialNumberRegexPattern = @"^\d{17}[0123456789X]$";

		public static bool IsInt32(this string? str) => int.TryParse(str, out _);
		public static bool IsInt64(this string? str) => long.TryParse(str, out _);
		public static bool IsNumeric(this string? str) => decimal.TryParse(str, out _);
		public static bool IsBoolean(this string? str) => bool.TryParse(str, out _);
		public static bool IsDateTime(this string? str) => DateTime.TryParse(str, out _);
		public static bool IsIPAddress(this string? str) => IPAddress.TryParse(str, out _);

		public static bool IsUrl(this string? str) => str != null && str.Length >= 6 && Uri.IsWellFormedUriString(str, UriKind.Absolute);
		public static bool IsEmail(this string? str) => str != null && str.Length >= 6 && Regex.IsMatch(str, EmailRegexPattern);
		public static bool IsCardNumber(this string? str) => str != null && (str.Length == 16 || str.Length == 19) && Regex.IsMatch(str, @"^\d+$");
		public static bool IsMainlandMobile(this string? str) => str != null && str.Length == 11 && Regex.IsMatch(str, MainlandMobileRegexPattern);

		public static bool IsMainlandSocialNumber(this string? str, Sex? sex = null, bool adultOnly = true) {
			if ( str == null || str.Length != 18 ) return false;
			if ( sex == Sex.Male && (str[16] - '0') % 2 == 0 ) return false;
			if ( sex == Sex.Female && (str[16] - '0') % 2 == 1 ) return false;
			if ( str.Substring(6, 4).AsInt() < 1930 ) return false;
			if ( str.Substring(6, 4).AsInt() > DateTime.Now.Year - 16 && adultOnly ) return false;
			if ( str.Substring(10, 2).AsInt() > 12 ) return false;
			if ( str.Substring(12, 2).AsInt() > 31 ) return false;
			var times = new[] { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };
			var n = 0;
			for ( var i = 0; i < 17; n += (str[i] - '0') * times[i++] ) ;
			return new[] { '1', '0', 'X', '9', '8', '7', '6', '5', '4', '3', '2' }[n % 11] == str[17];
		}
		public static Sex? GetSexFromMainlandSocialNumber(this string? str) {
			if ( str == null || !str.IsMainlandSocialNumber() ) {
				return null;
			}
			return (str[16] - '0') % 2 == 0 ? Sex.Female : Sex.Male;
		}
	}
}