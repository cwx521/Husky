using System;
using System.Net;
using System.Text.RegularExpressions;

namespace Husky
{
	public static class StringTest
	{
		public const string EmailRegexPattern = @"^[0-9a-zA-Z][-0-9a-zA-Z.+_]+@[-0-9a-zA-Z.+_]+\.[a-zA-Z]{2,4}$";
		public const string MobileNumberRegexPattern = @"^1[3456789]\d{9}$";

		public static bool IsInt(this string? str) => int.TryParse(str, out _);
		public static bool IsInt64(this string? str) => long.TryParse(str, out _);
		public static bool IsNumeric(this string? str) => decimal.TryParse(str, out _);
		public static bool IsBoolean(this string? str) => bool.TryParse(str, out _);
		public static bool IsDateTime(this string? str) => DateTime.TryParse(str, out _);
		public static bool IsIPAddress(this string? str) => IPAddress.TryParse(str, out _);
		public static bool IsUrl(this string? str) => str != null && str.Length >= 6 && Uri.IsWellFormedUriString(str, UriKind.Absolute);
		public static bool IsEmail(this string? str) => str != null && str.Length >= 6 && Regex.IsMatch(str, EmailRegexPattern);
		public static bool IsCardNumber(this string? str) => str != null && (str.Length == 16 || str.Length == 19) && Regex.IsMatch(str, @"^\d+$");
		public static bool IsMobileNumber(this string? str) => str != null && str.Length == 11 && Regex.IsMatch(str, MobileNumberRegexPattern);
		public static bool IsSocialIdNumber(this string? str) => str != null && new SocialIdNumber(str).IsValid;
	}
}