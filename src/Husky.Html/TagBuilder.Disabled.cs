using Microsoft.AspNetCore.Html;

namespace Husky.Html
{
	public static partial class HtmlHelperExtensions
	{
		public static IHtmlContent Disabled(this IHtmlContent tagBuilder, bool isDisabled = true) {
			return tagBuilder.Prop("disabled", isDisabled ? "disabled" : null);
		}
	}
}
