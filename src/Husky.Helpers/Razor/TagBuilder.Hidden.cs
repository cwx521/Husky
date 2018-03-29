using Microsoft.AspNetCore.Html;

namespace Husky.Razor
{
	public static partial class HtmlHelperExtensions
	{
		public static IHtmlContent Hidden(this IHtmlContent tagBuilder, bool isHidden = true) {
			return tagBuilder.Prop("hidden", isHidden ? "hidden" : null);
		}
	}
}
