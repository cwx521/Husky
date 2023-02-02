using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Husky.GridQuery
{
	public static class GridColumnBuilder
	{
		internal const int DefaultGridColumnWidth = 160;

		public static string Json(this List<GridColumn> columns) {
			return JsonSerializer.Serialize(columns, new JsonSerializerOptions(JsonSerializerDefaults.Web) {
				DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
			});
		}

		public static List<GridColumn> GetGridColumns<TGridModel>() => typeof(TGridModel).GetGridColumns();
		public static List<GridColumn> GetGridColumns(this Type typeOfGridModel) {
			if (typeOfGridModel == null) {
				throw new ArgumentNullException(nameof(typeOfGridModel));
			}

			var properties = typeOfGridModel.GetProperties();
			var columns = new List<GridColumn>();
			var pending = new List<Tuple<GridColumn, GridColumnAttribute>>();

			// read the properties and create columns one by one
			// when attribute.DisplayAfter is set, the columns will be added later on
			foreach (var p in properties) {
				var attribute = ResolveGridColumnAttribute(p);
				if (attribute != null && !attribute.Visible) {
					continue;
				}
				if (attribute != null && !string.IsNullOrEmpty(attribute.DisplayAfter)) {
					var tuple = new Tuple<GridColumn, GridColumnAttribute>(
						BuildGridColumn(p, attribute),
						attribute
					);
					pending.Add(tuple);
				}
				columns.Add(BuildGridColumn(p, attribute));
			}

			// now insert columns which have DisplayAfter
			foreach (var each in pending) {
				var insertAt = columns.FindIndex(x => x.Field == each.Item2.DisplayAfter) + 1;
				columns.Insert(insertAt, each.Item1);
			}

			// grouping
			var result = new List<GridColumn>();
			foreach (var col in columns) {
				if (string.IsNullOrEmpty(col.Category)) {
					result.Add(col);
					continue;
				}
				var category = result.Find(x => x.Title == col.Category);
				if (category == null) {
					category = new GridColumn {
						Title = col.Category,
						Columns = new List<GridColumn>()
					};
					result.Add(category);
				}
				category.Columns!.Add(col);
			}

			return result;
		}

		private static GridColumn BuildGridColumn(PropertyInfo property, GridColumnAttribute? attr) => new() {
			Field = property?.Name/*.CamelCase()*/,
			Template = attr?.Template ?? GetTemplateString(attr, property?.Name/*.CamelCase()*/),
			Title = attr?.Title ?? property?.Name?.SplitWordsByCapital(),
			Category = attr?.Category,
			Width = (attr == null || attr.Width == 0) ? DefaultGridColumnWidth : (attr.Width != -1 ? attr.Width : (int?)null),
			Format = attr?.Format,
			Aggregates = attr?.Aggregates.ToNameArray(),
			Filterable = property != null && (attr?.Filterable ?? true),
			Sortable = property != null && (attr?.Sortable ?? true),
			EditableFlag = property != null && (attr?.Editable ?? false),
			Hidable = (attr?.Hidable ?? true),
			Groupable = (attr?.Groupable ?? false),
			Locked = (attr?.Locked ?? false),
			Hidden = (attr?.Hidden ?? false),
			Attributes = new GridColumnTdAttributes { Class = attr?.CssClass, Style = attr?.MergeCssStyle() },
			Type = property?.GetMappedJavaScriptType(),
			Values = property?.GetEnumerableValues(),
		};

		private static GridColumnAttribute? ResolveGridColumnAttribute(PropertyInfo gridColumnProperty) {
			var result = gridColumnProperty.GetCustomAttribute<GridColumnAttribute>();
			var resultTypeProps = typeof(GridColumnAttribute).GetProperties();

			var attributes = gridColumnProperty.GetCustomAttributes();
			foreach (var dedicatedAttr in attributes) {
				if (dedicatedAttr is GridColumnAttribute) {
					continue;
				}

				var dedicatedAttrProperties = dedicatedAttr.GetType().GetProperties();
				foreach (var each in dedicatedAttrProperties) {
					if (each.Name == nameof(Attribute.TypeId)) {
						continue;
					}
					var value = each.GetValue(dedicatedAttr);
					if (value != default) {
						result ??= new GridColumnAttribute();
						resultTypeProps.SingleOrDefault(x => x.Name == each.Name)?.SetValue(result, value);
					}
				}
			}
			return result;
		}

		private static string? CamelCase(this string? fieldName) {
			if (string.IsNullOrEmpty(fieldName)) {
				return fieldName;
			}
			return fieldName.Substring(0, 1).ToLower() + fieldName[1..];
		}

		private static string? TrimBracers(this string? format) {
			return format?.Replace("{0:", "").TrimStart('{').TrimEnd('}');
		}

		private static string[]? ToNameArray(this Enum? values) {
			return values?.ToString().Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
		}

		private static string? MergeCssStyle(this GridColumnAttribute? attr) {
			if (attr == null) {
				return null;
			}
			if (attr.TextAlign != TextAlign.Start) {
				var align = attr.TextAlign == TextAlign.End ? "right" : "center";
				attr.CssStyle = $"text-align: {align};" + attr.CssStyle;
			}
			return attr.CssStyle;
		}

		private static string? GetMappedJavaScriptType(this PropertyInfo property) {
			var t = !property.PropertyType.IsGenericType
				? property.PropertyType
				: property.PropertyType.GenericTypeArguments[0];

			if (t == typeof(DateTime)) {
				return "date";
			}
			if (t == typeof(int) || t == typeof(long) || t == typeof(uint) || t == typeof(decimal) || t == typeof(double) || t == typeof(float)) {
				return "number";
			}
			return null;
		}

		private static SelectListItem[]? GetEnumerableValues(this PropertyInfo property) {
			var t = !property.PropertyType.IsGenericType
				? property.PropertyType
				: property.PropertyType.GenericTypeArguments[0];

			if (!t.IsEnum) {
				return null;
			}
			var result = new List<SelectListItem>();
			var values = Enum.GetValues(t);
			foreach (var i in values) {
				result.Add(new SelectListItem {
					Text = (i as Enum)!.ToLabel(),
					Value = i!.GetHashCode().ToString()
				});
			}
			return result.ToArray();
		}

		private static string? GetTemplateString(GridColumnAttribute? attr, string? fieldName) {
			if (attr == null || string.IsNullOrEmpty(fieldName)) {
				return null;
			}

			var nested = PrepareNestedTemplateString(attr, fieldName);

			if (string.IsNullOrEmpty(attr.LinkUrl)) {
				return nested;
			}

			nested ??= string.IsNullOrEmpty(attr.Format)
				? $"#: {fieldName} #"
				: $"#: kendo.toString({fieldName}, '{attr.Format.TrimBracers()}')#";

			switch (attr.LinkTarget) {
				default:
				case GridColumLinkTarget.Self:
					return $"#if ({fieldName}) {{# <a href='{attr.LinkUrl}'>{nested}</a> #}} #";

				case GridColumLinkTarget.NewWindow:
					return $"#if ({fieldName}) {{# <a href='{attr.LinkUrl}' target='_blank'>{nested}</a> #}} #";

				case GridColumLinkTarget.Modal:
				case GridColumLinkTarget.ModalLG:
				case GridColumLinkTarget.ModalXL:
				case GridColumLinkTarget.ModalFullScreen:
					var size = attr.LinkTarget.ToLower().Substring(5);
					var link = $"<a " +
									$"href='{attr.LinkUrl}' " +
									$"data-ajax='husky' " +
									$"data-toggle='modal' " +
									$"data-target='\\#modal' " +
									$"data-modal-size='modal-{size}'>" +
									$"{nested}" +
								$"</a>";
					return $"#if ({fieldName}) {{# {link} #}} #";
			}
		}

		private static string? PrepareNestedTemplateString(GridColumnAttribute? attr, string? fieldName) {
			if (attr == null || string.IsNullOrEmpty(fieldName)) {
				return null;
			}

			switch (attr.KnownTemplate) {
				default:
				case GridColumnTemplate.None:
					return null;

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
						: $"#: {fieldName} == 0 ? '' : kendo.toString({fieldName}, '{attr.Format.TrimBracers()}') #";
			}
		}
	}
}