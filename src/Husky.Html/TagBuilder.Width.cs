using Microsoft.AspNetCore.Html;

namespace Husky.Html
{
	public static partial class HtmlHelperExtensions
	{
		public static IHtmlContent Width(this IHtmlContent tagBuilder, int px) {
			return tagBuilder.Prop("style", $"width: {px}px");
		}

		public static IHtmlContent Width(this IHtmlContent tagBuilder, string width) {
			return tagBuilder.Prop("style", $"width: {width}");
		}
	}
}
