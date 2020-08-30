using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Husky
{
	public static class EnumHelper
	{
		public static string ToLower(this Enum value) => value.ToString().ToLower();
		public static string ToUpper(this Enum value) => value.ToString().ToUpper();
		public static string ToLabel(this Enum value) => value.GetLabel(useDescription: false, enableCss: false);
		public static string ToLabelWithCss(this Enum value) => value.GetLabel(useDescription: false, enableCss: true);
		public static string ToDescription(this Enum value) => value.GetLabel(useDescription: true, enableCss: false);
		public static string ToDescriptionWithCss(this Enum value) => value.GetLabel(useDescription: true, enableCss: true);

		public static List<SelectListItem> ToSelectListItems<TEnum>(string optionLabel = null, bool useIntValue = false) where TEnum : struct, IConvertible => ToSelectListItems(typeof(TEnum), optionLabel, useIntValue);
		public static List<SelectListItem> ToSelectListItems(Type enumType, string optionLabel = null, bool useIntValue = false) {
			var result = new List<SelectListItem>();

			foreach ( int value in Enum.GetValues(enumType) ) {
				var name = Enum.GetName(enumType, value);
				result.Add(new SelectListItem {
					Text = ((Enum)Enum.Parse(enumType, name)).ToLabel(),
					Value = useIntValue ? value.ToString() : name
				});
			}
			if ( enumType == typeof(YesNo) || enumType == typeof(OnOff) ) {
				result.Reverse();
			}
			if ( optionLabel != null ) {
				result.Insert(0, new SelectListItem {
					Text = optionLabel,
					Value = null
				});
			}
			return result;
		}


		private static string GetLabel(this Enum value, bool useDescription, bool enableCss) {
			var fieldName = Enum.GetName(value.GetType(), value);
			if ( fieldName != null ) {
				var field = value.GetType().GetField(fieldName);
				return DisplayAs(field, fieldName, useDescription, enableCss);
			}
			var result = string.Join(", ", GetMultipleLabels(value, useDescription, enableCss));
			return result == "0" ? "" : result;
		}

		private static IEnumerable<string> GetMultipleLabels(this Enum value, bool useDescription, bool enableCss) {
			var fieldNames = value.ToString().Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
			foreach ( var i in fieldNames ) {
				var field = value.GetType().GetField(i);
				yield return DisplayAs(field, i, useDescription, enableCss);
			}
		}

		private static string DisplayAs(FieldInfo field, string fieldName, bool useDescription, bool enableCss) {
			var attribute = field?.GetCustomAttribute<LabelAttribute>();
			var text = (useDescription ? attribute?.Description : null) ?? attribute?.Label ?? fieldName;
			return !enableCss ? text : $"<span class='{attribute.CssClass}'>{text}</span>";
		}
	}
}