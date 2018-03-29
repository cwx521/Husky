using Microsoft.AspNetCore.Html;

namespace Husky.Razor
{
	public static partial class HtmlHelperExtensions
	{
		public static IHtmlContent Width(this IHtmlContent tagBuilder, int px) {
			return tagBuilder.Prop("style", $"width: {px}px");
		}

		public static IHtmlContent Width(this IHtmlContent tagBuilder, string width) {
			return tagBuilder.Prop("style", $"width: {width}");
		}

		public static IHtmlContent Short(this IHtmlContent tagBuilder) {
			return tagBuilder.AddCssClass("form-control-short");
		}

		public static IHtmlContent Shortest(this IHtmlContent tagBuilder) {
			return tagBuilder.AddCssClass("form-control-shortest");
		}
	}
}
