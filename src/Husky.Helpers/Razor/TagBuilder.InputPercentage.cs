using System;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Husky.Razor
{
	public static partial class HtmlHelperExtensions
	{
		public static IHtmlContent InputPercentage(this IHtmlContent tagBuilder, int scale = 2) {
			if ( tagBuilder == null ) {
				return null;
			}
			if ( tagBuilder is TagBuilder ctl ) {
				var id = ctl.Attributes["id"];
				var name = ctl.Attributes["name"];
				var valueString = ctl.Attributes["value"];
				var hidden = $"<input type='hidden' name='{name}' value='{valueString}' />";

				decimal.TryParse(valueString, out var value);
				ctl.Attributes["value"] = (value * 100).ToString("f" + scale).TrimEnd('0').TrimEnd('.');
				ctl.Attributes["name"] = "_dummy_" + name;

				var result = new HtmlContentBuilder();
				result.AppendHtml("<div class='input-group input-group-percentage'>");
				result.AppendHtml(ctl);
				result.AppendHtml("  <div class='input-group-addon'>%</div>");
				result.AppendHtml("</div>");
				result.AppendHtml(hidden);

				result.AppendHtml("<script type='text/javascript'>");
				result.AppendHtml($" $('#{id}').change(function() {{");
				result.AppendHtml("    $(this.parentNode.nextSibling).val(parseFloat($(this).val()) / 100);");
				result.AppendHtml($" }});");
				result.AppendHtml("</script>");

				return result;
			}
			throw new InvalidCastException($"The type of the parameter is not TagBuilder.");
		}
	}
}
