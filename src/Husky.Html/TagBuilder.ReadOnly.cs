using Microsoft.AspNetCore.Html;

namespace Husky.Html
{
	public static partial class HtmlHelperExtensions
	{
		public static IHtmlContent ReadOnly(this IHtmlContent tagBuilder, bool isReadOnly = true) {
			return tagBuilder.Prop("readonly", isReadOnly ? "readonly" : null);
		}
	}
}
