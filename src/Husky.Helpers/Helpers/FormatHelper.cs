using System;

namespace Husky
{
	public static class FormatHelper
	{
		public static string DateFormat { get; set; } = "yyyy年M月d日";
		public static string TimeFormat { get; set; } = "HH:mm:ss";
		public static string DateTimeFormat { get; set; } = "yyyy年M月d日 HH:mm:ss";
		public static string ShortDateFormat { get; set; } = "M月d日";
		public static string ShortTimeFormat { get; set; } = "HH:mm";
		public static string ShortDateTimeFormat { get; set; } = "M月d日 HH:mm";

		public static bool IsToday(this DateTime datetime) => datetime.Date == DateTime.Today;
		public static string ToString(this DateTime? datetime, string format) => datetime.HasValue ? datetime.Value.ToString(format) : string.Empty;

		public static string ToDateString(this DateTime datetime, string format = null) => datetime.ToString(format ?? DateFormat);
		public static string ToDateString(this DateTime? datetime, string format = null) => datetime.ToString(format ?? DateFormat);
		public static string ToTimeString(this DateTime datetime, string format = null) => datetime.ToString(format ?? TimeFormat);
		public static string ToTimeString(this DateTime? datetime, string format = null) => datetime.ToString(format ?? TimeFormat);
		public static string ToDateTimeString(this DateTime datetime, string format = null) => datetime.ToString(format ?? DateTimeFormat);
		public static string ToDateTimeString(this DateTime? datetime, string format = null) => datetime.ToString(format ?? DateTimeFormat);

		public static string ToShortDateString(this DateTime datetime, string format = null) => datetime.ToString(format ?? ShortDateFormat);
		public static string ToShortDateString(this DateTime? datetime, string format = null) => datetime.ToString(format ?? ShortDateFormat);
		public static string ToShortTimeString(this DateTime datetime, string format = null) => datetime.ToString(format ?? ShortTimeFormat);
		public static string ToShortTimeString(this DateTime? datetime, string format = null) => datetime.ToString(format ?? ShortTimeFormat);
		public static string ToShortDateTimeString(this DateTime datetime, string format = null) => datetime.ToString(format ?? ShortDateTimeFormat);
		public static string ToShortDateTimeString(this DateTime? datetime, string format = null) => datetime.ToString(format ?? ShortDateTimeFormat);

		public static string TrimEnd(this decimal? d, string format = null) => d.ToString(format).TrimEnd('0').TrimEnd('.');
		public static string TrimEnd(this float? f, string format = null) => f.ToString(format).TrimEnd('0').TrimEnd('.');
		public static string TrimEnd(this double? d, string format = null) => d.ToString(format).TrimEnd('0').TrimEnd('.');

		public static string ToString(this int? value, string format) => value.HasValue ? value.Value.ToString(format) : string.Empty;
		public static string ToString(this uint? value, string format) => value.HasValue ? value.Value.ToString(format) : string.Empty;
		public static string ToString(this long? value, string format) => value.HasValue ? value.Value.ToString(format) : string.Empty;
		public static string ToString(this decimal? value, string format) => value.HasValue ? value.Value.ToString(format) : string.Empty;
		public static string ToString(this float? value, string format) => value.HasValue ? value.Value.ToString(format) : string.Empty;
		public static string ToString(this double? value, string format) => value.HasValue ? value.Value.ToString(format) : string.Empty;
	}
}