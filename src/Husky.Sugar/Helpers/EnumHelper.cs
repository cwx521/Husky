using System;
using System.Collections.Generic;
using System.Reflection;

namespace Husky.Sugar
{
	public static class EnumHelper
	{
		public static string ToLower(this Enum value) => value.ToString().ToLower();
		public static string ToUpper(this Enum value) => value.ToString().ToUpper();
		public static string ToLabel(this Enum value) => value.GetLabel(useDescription: false);
		public static string ToDescription(this Enum value) => value.GetLabel(useDescription: true);

		private static string GetLabel(this Enum value, bool useDescription) {
			var fieldName = Enum.GetName(value.GetType(), value);
			if ( fieldName != null ) {
				var field = value.GetType().GetField(fieldName);
				var attribute = field?.GetCustomAttribute<LabelAttribute>();
				return (useDescription ? attribute?.Description : null) ?? attribute?.Label ?? fieldName;
			}
			return string.Join(", ", GetMultipleLabels(value, useDescription));
		}

		private static string[] GetMultipleLabels(this Enum value, bool useDescription) {
			var fieldNames = value.ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			var results = new List<string>();

			foreach ( var i in fieldNames ) {
				var field = value.GetType().GetField(i);
				var attribute = field?.GetCustomAttribute<LabelAttribute>();
				results.Add((useDescription ? attribute?.Description : null) ?? attribute?.Label ?? i);
			}
			return results.ToArray();
		}
	}
}