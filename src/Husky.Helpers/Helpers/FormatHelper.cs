using System;

namespace Husky
{
	public static class FormatHelper
	{
		public static string DateFormat { get; set; } = "dd/MMM/yyyy";
		public static string DateTimeFormat { get; set; } = "dd/MMM/yyyy H:mm:ss";

		public static string ToString(this DateTime? nullableDateTime, string format) => nullableDateTime.HasValue ? nullableDateTime.Value.ToString(format) : string.Empty;
		public static string ToString(this decimal? nullableValue, string format) => nullableValue.HasValue ? nullableValue.Value.ToString(format) : string.Empty;
		public static string ToString(this int? nullableValue, string format) => nullableValue.HasValue ? nullableValue.Value.ToString(format) : string.Empty;

		public static string ToDateString(this DateTime datetime, string format = null) => datetime.ToString(format ?? DateFormat);
		public static string ToDateString(this DateTime? nullableDateTime, string format = null) => nullableDateTime.ToString(format ?? DateFormat);
		public static string ToDateTimeString(this DateTime datetime, string format = null) => datetime.ToString(format ?? DateTimeFormat);
		public static string ToDateTimeString(this DateTime? nullableDateTime, string format = null) => nullableDateTime.ToString(format ?? DateTimeFormat);
	}
}