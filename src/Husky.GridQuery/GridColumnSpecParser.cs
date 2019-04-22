using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace Husky.GridQuery
{
	public static class GridColumnSpecParser
	{
		private const int GridColumnDefaultWidth = 160;

		public static string GetGridColumnJson(this Type typeOfGridModel) => JsonConvert.SerializeObject(typeOfGridModel.GetGridColumnSpecs());
		public static string GetGridColumnJson<TGridModel>() => typeof(TGridModel).GetGridColumnJson();


		public static List<GridColumnSpec> GetGridColumnSpecs<TGridModel>() => typeof(TGridModel).GetGridColumnSpecs();
		public static List<GridColumnSpec> GetGridColumnSpecs(this Type typeOfGridModel) {
			if ( typeOfGridModel == null ) {
				throw new ArgumentNullException(nameof(typeOfGridModel));
			}

			var properties = typeOfGridModel.GetProperties();
			var columns = new List<GridColumnSpec>();

			// if [GridColumnCheckBox] attribute was declared at the model type, create a checkbox column first.
			var checkBoxColumnAttribute = typeOfGridModel.GetCustomAttribute<GridColumnCheckBoxAttribute>(false);
			if ( checkBoxColumnAttribute != null ) {
				columns.Add(BuildGridColumnSpecModel(null, checkBoxColumnAttribute));
			}

			// read the properties and create columns one by one
			// when attribute.DisplayAfter is set, the columns will be added later on
			foreach ( var p in properties ) {
				if ( p.Name.Equals("Id") || p.Name.EndsWith("Css") || p.Name == "IsHighlighted" || p.Name == "IsMuted" ) {
					continue;
				}
				var attribute = p.GetCustomAttribute<GridColumnAttribute>(true);
				if ( attribute != null && (!attribute.Visible || !string.IsNullOrEmpty(attribute.DisplayAfter)) ) {
					continue;
				}
				columns.Add(BuildGridColumnSpecModel(p, attribute));
			}

			// now insert columns which have DisplayAfter
			foreach ( var p in properties ) {
				var attribute = p.GetCustomAttribute<GridColumnAttribute>(true);
				if ( attribute == null || string.IsNullOrEmpty(attribute.DisplayAfter) ) {
					continue;
				}

				var column = BuildGridColumnSpecModel(p, attribute);
				var insertAt = columns.GetColumnDisplayIndex(attribute.DisplayAfter);
				columns.Insert(insertAt, column);
			}

			// grouping
			var result = new List<GridColumnSpec>();
			foreach ( var c in columns ) {
				if ( string.IsNullOrEmpty(c.group) ) {
					result.Add(c);
					continue;
				}
				var g = result.SingleOrDefault(x => x.title == c.group);
				if ( g == null ) {
					g = new GridColumnSpec {
						title = c.group,
						columns = new List<GridColumnSpec>()
					};
					result.Add(g);
				}
				g.columns.Add(c);
			}

			return result;
		}

		private static GridColumnSpec BuildGridColumnSpecModel(PropertyInfo property, GridColumnAttribute attr) {
			var column = new GridColumnSpec {
				field = property?.Name,
				title = attr?.Title ?? property?.Name?.SplitWords(),
				type = property?.GetMappedJsType(),
				group = attr?.Group,
				width = (attr != null && attr.Width != 0) ? attr.Width : GridColumnDefaultWidth,
				template = attr?.Template ?? GetTemplateString(attr, property?.Name),
				format = attr?.Format,
				sortable = (attr?.Sortable ?? true),
				locked = (attr?.Locked ?? false),
				editableFlag = (attr?.Editable ?? true),
				hidable = (attr?.Hidable ?? true),
				hidden = (attr?.Hidden ?? false),
				attributes = attr?.CssClass == null ? null : $" class='{attr.CssClass}'",
				values = property?.TryGetEnumItems(),
				filterable = property != null && (attr?.Filterable ?? true)
			};
			if ( column.width == -1 ) {
				column.width = null;
			}
			return column;
		}

		private static string GetMappedJsType(this PropertyInfo property) {
			if ( property == null ) {
				return null;
			}
			if ( property.PropertyType == typeof(DateTime) ||
				 property.PropertyType == typeof(DateTime?) ) {
				return "date";
			}
			if ( property.PropertyType == typeof(int) ||
				 property.PropertyType == typeof(int?) ||
				 property.PropertyType == typeof(decimal) ||
				 property.PropertyType == typeof(decimal?) ||
				 property.PropertyType == typeof(double) ||
				 property.PropertyType == typeof(double?) ||
				 property.PropertyType == typeof(float) ||
				 property.PropertyType == typeof(float?) ) {
				return "number";
			}
			return null;
		}

		private static int GetColumnDisplayIndex(this List<GridColumnSpec> columns, string fieldName) => columns.IndexOf(columns.Find(x => x.field == fieldName)) + 1;

		private static GridColumnSpecEnumItem[] TryGetEnumItems(this PropertyInfo property) {
			var t = !property.PropertyType.IsGenericType ? property.PropertyType : property.PropertyType.GenericTypeArguments[0];
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
					return $"<a href='{attr.Url}'>#:{fieldName}#</a>";

				case GridColumnTemplate.Modal:
				case GridColumnTemplate.ModalLG:
				case GridColumnTemplate.ModalXL:
				case GridColumnTemplate.ModalFull:
					var size = attr.KnownTemplate.ToLower().Substring(5);
					return $"<a href='{attr.Url}' data-toggle='modal' " +
								$"data-target='\\#modal' " +
								$"data-modal-footer='false' " +
								$"data-modal-size='modal-{size}' " +
								$"data-modal-title='#:{fieldName} || '{fieldName}'#'>" +
								$"#:{fieldName}#" +
								$"</a>";

				case GridColumnTemplate.Date:
					return $"#: kendo.toString({fieldName}, 'yyyy-M-d') || '' #";

				case GridColumnTemplate.DateTime:
					return $"#: kendo.toString({fieldName}, 'yyyy-M-d H:mm') || '' #";

				case GridColumnTemplate.TimeAgo:
					return $"#: ago({fieldName}) || '' #";

				case GridColumnTemplate.Elapsed:
				case GridColumnTemplate.ElapsedSeconds:
					return $"#: elapsed({fieldName}) || '' #";

				case GridColumnTemplate.CheckBox:
					var sb = new StringBuilder();
					sb.Append("<input type='checkbox' class='grid-row-checkbox' ");
					sb.AppendFormat("#{0} == {1} ? 'checked' : ''# ", fieldName, (int)CheckBoxState.Checked);
					sb.AppendFormat("#{0} == {1} ? 'disabled' : ''# ", fieldName, (int)CheckBoxState.Disabled);
					sb.AppendFormat("name='idCollection' value='#:Id#'");
					sb.Append("/>");
					return $"#if ({fieldName} != {(int)CheckBoxState.NoDisplay}) {{# {sb.ToString()} #}}#";

				case GridColumnTemplate.ZeroAsEmpty:
					return $"#: {fieldName} == 0 ? '' : {fieldName} #";

				default:
					return null;
			}
		}
	}
}