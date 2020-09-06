using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace Husky.GridQuery
{
	public static class GridColumnSpecParser
	{
		private const int GridColumnDefaultWidth = 160;

		public static string GetGridColumnJson<TGridModel>() => typeof(TGridModel).GetGridColumnJson();
		public static string GetGridColumnJson(this Type typeOfGridModel) => JsonConvert.SerializeObject(typeOfGridModel.GetGridColumnSpecs());

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
				if ( p.Name.Equals("Id") || p.Name.EndsWith("Css") || p.Name == "IsHighlighted" || p.Name == "IsMuted" ) {
					continue;
				}
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
				var insertAt = columns.FindIndex(x => x.field == attribute.DisplayAfter) + 1;
				columns.Insert(insertAt, column);
			}

			// grouping
			var result = new List<GridColumnSpec>();
			foreach ( var col in columns ) {
				if ( string.IsNullOrEmpty(col.group) ) {
					result.Add(col);
					continue;
				}
				var g = result.SingleOrDefault(x => x.title == col.group);
				if ( g == null ) {
					g = new GridColumnSpec {
						title = col.group,
						columns = new List<GridColumnSpec>()
					};
					result.Add(g);
				}
				g.columns.Add(col);
			}

			return result;
		}

		private static GridColumnSpec BuildGridColumnSpecModel(PropertyInfo property, GridColumnAttribute attr) => new GridColumnSpec {
			field = property?.Name,
			title = attr?.Title ?? property?.Name?.SplitWords(),
			group = attr?.Group,
			width = (attr == null || attr.Width == 0) ? GridColumnDefaultWidth : (attr.Width != -1 ? attr.Width : (int?)null),
			template = attr?.Template ?? GetTemplateString(attr, property?.Name),
			format = attr?.Format,
			filterable = property != null && (attr?.Filterable ?? true),
			sortable = (attr?.Sortable ?? true),
			hidable = (attr?.Hidable ?? true),
			editableFlag = (attr?.Editable ?? true),
			locked = (attr?.Locked ?? false),
			hidden = (attr?.Hidden ?? false),
			attributes = attr?.CssClass == null ? null : $" class='{attr.CssClass}'",
			type = property?.GetMappedJsType(),
			values = property?.GetEnumerableValues(),
		};

		private static string GetMappedJsType(this PropertyInfo property) {
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

		private static GridColumnSpecEnumItem[] GetEnumerableValues(this PropertyInfo property) {
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
					text = EnumHelper.ToLabel(i as Enum),
					value = i.GetHashCode()
				});
			}
			return result.ToArray();
		}

		private static string GetTemplateString(GridColumnAttribute attr, string fieldName) {
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