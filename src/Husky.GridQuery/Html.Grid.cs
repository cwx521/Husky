using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Husky.GridQuery
{
	public static partial class HtmlHelperExtensions
	{
		public static IHtmlContent Grid<TGridModel>(this IHtmlHelper helper,
			string dataSourceUrl,
			QueryCriteria? criteria = null,
			GridEditable editable = GridEditable.NA,
			Action<List<GridColumn>>? customize = null) {

			return helper.Grid(typeof(TGridModel), dataSourceUrl, criteria, editable, customize);
		}

		public static IHtmlContent Grid(this IHtmlHelper helper,
			Type typeOfGridModel,
			string dataSourceUrl,
			QueryCriteria? criteria = null,
			GridEditable editable = GridEditable.NA,
			Action<List<GridColumn>>? customize = null) {

			var columns = typeOfGridModel.GetGridColumns();
			customize?.Invoke(columns);

			var id = "g" + Crypto.RandomString();
			var serializedCriteria = (criteria ?? new QueryCriteria()).Json();
			var serializedColumns = columns.Json();
			var sb = new StringBuilder();
			var script = $"$('#{id}').loadGrid('{dataSourceUrl}', {serializedCriteria}, {serializedColumns}, {(int)editable})";

			sb.AppendLine("<div id='" + id + "' style='height: 100%'>");
			sb.AppendLine("<script type='text/javascript'>");
			sb.AppendLine("	 $(function() {" + script + "});");
			sb.AppendLine("</script>");
			sb.AppendLine("</div>");

			return new HtmlString(sb.ToString());
		}
	}
}