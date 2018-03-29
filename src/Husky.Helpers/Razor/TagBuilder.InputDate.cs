using Microsoft.AspNetCore.Html;

namespace Husky.Razor
{
	public static partial class HtmlHelperExtensions
	{
		public static IHtmlContent InputDate(this IHtmlContent tagBuilder) {
			return tagBuilder.AddCssClass("form-datepicker");
		}
	}
}
