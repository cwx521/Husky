using Microsoft.AspNetCore.Html;

namespace Husky.Html
{
	public static partial class HtmlHelperExtensions
	{
		public static IHtmlContent Select2(this IHtmlContent tagBuilder) {
			return tagBuilder.AddCssClass("select2");
		}

		public static IHtmlContent Combo(this IHtmlContent tagBuilder, bool enableCombo = true) {
			return enableCombo ? tagBuilder.Prop("data-tags", "true") : tagBuilder;
		}

		public static IHtmlContent Multiple(this IHtmlContent tagBuilder, bool enableMultiple = true) {
			return enableMultiple ? tagBuilder.Prop("data-multiple", "true") : tagBuilder;
		}

		public static IHtmlContent DeferLoadFrom(this IHtmlContent tagBuilder, string dataSourceUrl) {
			return tagBuilder.Prop("data-src", dataSourceUrl);
		}
	}
}
