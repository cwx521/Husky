using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Husky.Principal;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.GridQuery
{
	public static partial class HtmlHelperExtensions
	{
		public static IHtmlContent Grid<TGridModel>(this IHtmlHelper helper, string dataSourceUrl, QueryCriteria? criteria = null, GridEditable editable = GridEditable.NA, Action<List<GridColumnSpec>>? customize = null) => helper.Grid(typeof(TGridModel), dataSourceUrl, criteria, editable, customize);
		public static IHtmlContent Grid(this IHtmlHelper helper, Type typeOfGridModel, string dataSourceUrl, QueryCriteria? criteria = null, GridEditable editable = GridEditable.NA, Action<List<GridColumnSpec>>? customize = null) {
			var principal = helper.ViewContext.HttpContext.RequestServices.GetRequiredService<IPrincipalUser>();
			var cookies = helper.ViewContext.HttpContext.Request.Cookies;

			var columns = typeOfGridModel.GetGridColumnSpecs();
			customize?.Invoke(columns);

			if ( cookies.TryGetValue(principal.GetGridCookieKey(dataSourceUrl, "Hide"), out var hiddenColumns) ) {
				var arr = hiddenColumns.Split(',');
				foreach ( var col in columns ) {
					col.Hidden = arr.Contains(col.Field);
				}
			}
			if ( cookies.TryGetValue(principal.GetGridCookieKey(dataSourceUrl, "Order"), out var columnOrders) ) {
				var i = 0;
				var arr = columnOrders.Split(',');
				foreach ( var str in arr ) {
					var col = columns.Find(x => x.Field == str);
					if ( col != null ) {
						columns.Insert(i, col);
						columns.RemoveAt(columns.FindLastIndex(x => x.Field == str));
						i++;
					}
				}
			}

			var id = "g" + Crypto.RandomString();
			var criteriaJson = (criteria ?? new QueryCriteria()).Json();
			var columnsJson = columns.Json();
			var sb = new StringBuilder();

			sb.AppendLine($"<div id='{id}' style='height: 100%'>");
			sb.AppendLine("<script type='text/javascript'>");
			sb.AppendLine("	$(function () {");
			sb.AppendLine($"	$('#{id}').loadGrid('{dataSourceUrl}', {criteriaJson}, {columnsJson}, {(int)editable});");
			sb.AppendLine("	});");
			sb.AppendLine("</script>");
			sb.AppendLine("</div>");

			return new HtmlString(sb.ToString());
		}

		public static string GetGridCookieKey(this IPrincipalUser me, string dataSourceUrl, string forAction) {
			return Crypto
				.MD5(dataSourceUrl + "|" + forAction + me.Id)
				.Substring(6, 6);
		}
	}
}