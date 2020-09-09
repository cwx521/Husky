using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace Husky.GridQuery
{
	public static class GridColumnSpecHelper
	{
		private const int GridColumnDefaultWidth = 160;

		public static string Json(this List<GridColumnSpec> specs) {
			return JsonConvert.SerializeObject(specs, new JsonSerializerSettings {
				DefaultValueHandling = DefaultValueHandling.Ignore
			});
		}

		public static List<GridColumnSpec> GetGridColumnSpecs<TGridModel>() => typeof(TGridModel).GetGridColumnSpecs();
		public static List<GridColumnSpec> GetGridColumnSpecs(this Type typeOfGridModel) {
			if ( typeOfGridModel == null ) {
				throw new ArgumentNullException(nameof(typeOfGridModel));
			}

			var properties = typeOfGridModel.GetProperties();
			var columns = new List<GridColumnSpec>();

			// read the properties and create columns one by one
			// when attribute.DisplayAfter is set, the columns will be added later on
			foreach ( var p in properties ) {
				var attribute = p.GetCustomAttribute<GridColumnAttribute>();
				if ( attribute != null && (!attribute.Visible || !string.IsNullOrEmpty(attribute.DisplayAfter)) ) {
					continue;
				}
				columns.Add(BuildGridColumnSpecModel(p, attribute));
			}

			// now insert columns which have DisplayAfter
			foreach ( var p in properties ) {
				var attribute = p.GetCustomAttribute<GridColumnAttribute>();
				if ( attribute == null || string.IsNullOrEmpty(attribute.DisplayAfter) ) {
					continue;
				}

				var column = BuildGridColumnSpecModel(p, attribute);
				var insertAt = columns.FindIndex(x => x.Field == attribute.DisplayAfter) + 1;
				columns.Insert(insertAt, column);
			}

			// grouping
			var result = new List<GridColumnSpec>();
			foreach ( var col in columns ) {
				if ( string.IsNullOrEmpty(col.Gather) ) {
					result.Add(col);
					continue;
				}
				var g = result.SingleOrDefault(x => x.Title == col.Gather);
				if ( g == null ) {
					g = new GridColumnSpec {
						Title = col.Gather,
						Columns = new List<GridColumnSpec>()
					};
					result.Add(g);
				}
				g.Columns!.Add(col);
			}

			return result;
		}

		private static GridColumnSpec BuildGridColumnSpecModel(PropertyInfo property, GridColumnAttribute? attr) => new GridColumnSpec {
			Field = property?.Name.CamelCase(),
			Title = attr?.Title ?? property?.Name?.SplitWords(),
			Gather = attr?.Gather,
			Width = (attr == null || attr.Width == 0) ? GridColumnDefaultWidth : (attr.Width != -1 ? attr.Width : (int?)null),
			Template = attr?.Template ?? GetTemplateString(attr, property?.Name),
			Format = attr?.Format,
			Aggregates = attr?.Aggregates?.ToNameArray(),
			Filterable = property != null && (attr?.Filterable ?? true),
			Sortable = property != null && (attr?.Sortable ?? true),
			EditableFlag = property != null && (attr?.Editable ?? true),
			Hidable = (attr?.Hidable ?? true),
			Groupable = (attr?.Groupable ?? false),
			Locked = (attr?.Locked ?? false),
			Hidden = (attr?.Hidden ?? false),
			Attributes = attr?.CssClass == null ? null : $" class='{attr.CssClass}'",
			Type = property?.GetMappedJavaScriptType(),
			Values = property?.GetEnumerableValues(),
		};

		private static string? CamelCase(this string? fieldName) {
			if ( string.IsNullOrEmpty(fieldName) ) {
				return fieldName;
			}
			return fieldName[0].ToString() + fieldName[1..];
		}

		private static string[]? ToNameArray(this Enum? values) {
			return values?.ToString().Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
		}

		private static string? GetMappedJavaScriptType(this PropertyInfo property) {
			var t = !property.PropertyType.IsGenericType
				? property.PropertyType
				: property.PropertyType.GenericTypeArguments[0];

			if ( t == typeof(DateTime) ) {
				return "date";
			}
			if ( t == typeof(int) || t == typeof(uint) || t == typeof(decimal) || t == typeof(double) || t == typeof(float) ) {
				return "number";
			}
			return null;
		}

		private static GridColumnSpecEnumItem[]? GetEnumerableValues(this PropertyInfo property) {
			var t = !property.PropertyType.IsGenericType
				? property.PropertyType
				: property.PropertyType.GenericTypeArguments[0];

			if ( !t.IsEnum ) {
				return null;
			}
			var result = new List<GridColumnSpecEnumItem>();
			var values = Enum.GetValues(t);
			foreach ( var i in values ) {
				result.Add(new GridColumnSpecEnumItem {
					Text = (i as Enum)!.ToLabel(),
					Value = i!.GetHashCode()
				});
			}
			return result.ToArray();
		}

		private static string? GetTemplateString(GridColumnAttribute? attr, string? fieldName) {
			if ( attr == null || string.IsNullOrEmpty(fieldName) ) {
				return null;
			}
			if ( !string.IsNullOrEmpty(attr.Url) && attr.KnownTemplate == GridColumnTemplate.None ) {
				attr.KnownTemplate = GridColumnTemplate.HyperLink;
			}
			switch ( attr.KnownTemplate ) {
				case GridColumnTemplate.HyperLink:
					return $"#if ({fieldName} {{# <a href='{attr.Url}'>#:{fieldName}#</a> #}} )#";

				case GridColumnTemplate.Modal:
				case GridColumnTemplate.ModalLG:
				case GridColumnTemplate.ModalXL:
				case GridColumnTemplate.ModalFull:
					var size = attr.KnownTemplate.ToLower().Substring(5);
					var fragment1 = $"<a " +
						$"href='{attr.Url}' " +
						$"data-toggle='modal' " +
						$"data-target='\\#modal' " +
						$"data-modal-title='#:{fieldName} || '{fieldName}'#'" +
						$"data-modal-size='modal-{size}'>" +
						$"#:{fieldName}#" +
					$"</a>";
					return $"#if ({fieldName} {{# {fragment1} #}} )#";

				case GridColumnTemplate.CheckBox:
					var fragment2 = "<input " +
						$"type='checkbox' name='idCollection' value='#:Id#' class='grid-row-checkbox' " +
						$"# {fieldName} == {(int)CheckBoxState.Checked} ? 'checked' : '' # " +
						$"# {fieldName} == {(int)CheckBoxState.Disabled} ? 'disabled' : '' # " +
					"/>";
					return $"#if ({fieldName} !== {(int)CheckBoxState.NoDisplay}) {{# {fragment2} #}} #";

				case GridColumnTemplate.Date:
					return $"#: $.toDateString({fieldName}) #";

				case GridColumnTemplate.DateTime:
					return $"#: $.toDateTimeString({fieldName}) #";

				case GridColumnTemplate.TimeAgo:
					return $"#: $.toTimeAgoString({fieldName}) #";

				case GridColumnTemplate.TimeElapsed:
					return $"#: $.toTimeElapsedString({fieldName}) #";

				case GridColumnTemplate.ZeroAsEmpty:
					return string.IsNullOrEmpty(attr.Format)
						? $"#: {fieldName} == 0 ? '' : {fieldName} #"
						: $"#: {fieldName} == 0 ? '' : kendo.toString({fieldName}, '{attr.Format}') #";

				case GridColumnTemplate.None:
				default:
					return null;
			}
		}
	}
}