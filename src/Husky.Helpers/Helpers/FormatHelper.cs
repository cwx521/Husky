using System;

namespace Husky
{
	public static class FormatHelper
	{
		public static long Timestamp(this DateTime datetime) => (datetime.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
		public static DateTime FromTimestamp(long timestamp) => new DateTime((timestamp * 10000000) + 621355968000000000).ToLocalTime();
		public static bool IsToday(this DateTime datetime) => datetime.Date == DateTime.Today;
		public static string ToString(this DateTime? datetime, string format) => datetime.HasValue ? datetime.Value.ToString(format) : string.Empty;

		public static string TrimEnd(this decimal d, string? format = null) => d.ToString(format).TrimEnd('0').TrimEnd('.');
		public static string TrimEnd(this float f, string? format = null) => f.ToString(format).TrimEnd('0').TrimEnd('.');
		public static string TrimEnd(this double d, string? format = null) => d.ToString(format).TrimEnd('0').TrimEnd('.');
		public static string TrimEnd(this decimal? d, string? format = null) => d.ToString(format).TrimEnd('0').TrimEnd('.');
		public static string TrimEnd(this float? f, string? format = null) => f.ToString(format).TrimEnd('0').TrimEnd('.');
		public static string TrimEnd(this double? d, string? format = null) => d.ToString(format).TrimEnd('0').TrimEnd('.');

		public static string ToString(this int? value, string? format) => value.HasValue ? value.Value.ToString(format) : string.Empty;
		public static string ToString(this uint? value, string? format) => value.HasValue ? value.Value.ToString(format) : string.Empty;
		public static string ToString(this long? value, string? format) => value.HasValue ? value.Value.ToString(format) : string.Empty;
		public static string ToString(this decimal? value, string? format) => value.HasValue ? value.Value.ToString(format) : string.Empty;
		public static string ToString(this float? value, string? format) => value.HasValue ? value.Value.ToString(format) : string.Empty;
		public static string ToString(this double? value, string? format) => value.HasValue ? value.Value.ToString(format) : string.Empty;
	}
}