using System;

namespace Husky
{
	public static class FormatHelper
	{
		public static string DateFormat { get; set; } = "yyyy年M月d日";
		public static string TimeFormat { get; set; } = "HH点mm分";
		public static string DateTimeFormat { get; set; } = "yyyy年M月d日 HH点mm分";
		public static string ShortDateFormat { get; set; } = "M月d日";
		public static string ShortTimeFormat { get; set; } = "HH:mm";
		public static string ShortDateTimeFormat { get; set; } = "M月d日 HH:mm";

		public static bool IsToday(this DateTime datetime) => datetime.Date == DateTime.Today;
		public static string ToString(this DateTime? nullableDateTime, string format) => nullableDateTime.HasValue ? nullableDateTime.Value.ToString(format) : string.Empty;
		public static string ToDateString(this DateTime datetime, string format = null) => datetime.ToString(format ?? DateFormat);
		public static string ToDateString(this DateTime? nullableDateTime, string format = null) => nullableDateTime.ToString(format ?? DateFormat);
		public static string ToDateTimeString(this DateTime datetime, string format = null) => datetime.ToString(format ?? DateTimeFormat);
		public static string ToDateTimeString(this DateTime? nullableDateTime, string format = null) => nullableDateTime.ToString(format ?? DateTimeFormat);
		public static string ToShortDateTimeString(this DateTime datetime, string format = null) => datetime.ToString(format ?? ShortDateTimeFormat);
		public static string ToShortDateTimeString(this DateTime? nullableDateTime, string format = null) => nullableDateTime.ToString(format ?? ShortDateTimeFormat);

		public static string TrimEnd(this decimal d, string format = null) => d.ToString(format).TrimEnd('0').TrimEnd('.');
		public static string TrimEnd(this float f, string format = null) => f.ToString(format).TrimEnd('0').TrimEnd('.');
		public static string TrimEnd(this double d, string format = null) => d.ToString(format).TrimEnd('0').TrimEnd('.');

		public static string ToString(this int? nullableValue, string format) => nullableValue.HasValue ? nullableValue.Value.ToString(format) : string.Empty;
		public static string ToString(this decimal? nullableValue, string format) => nullableValue.HasValue ? nullableValue.Value.ToString(format) : string.Empty;
		public static string ToString(this float? nullableValue, string format) => nullableValue.HasValue ? nullableValue.Value.ToString(format) : string.Empty;
		public static string ToString(this double? nullableValue, string format) => nullableValue.HasValue ? nullableValue.Value.ToString(format) : string.Empty;
	}
}