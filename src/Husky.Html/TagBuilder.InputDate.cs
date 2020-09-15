using Microsoft.AspNetCore.Html;

namespace Husky.Html
{
	public static partial class HtmlHelperExtensions
	{
		public static IHtmlContent InputDate(this IHtmlContent tagBuilder) {
			return tagBuilder.Prop("data-toggle", "datepicker");
		}
	}
}
