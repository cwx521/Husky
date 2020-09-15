using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Husky.Html
{
	public enum ButtonType
	{
		Button,
		Submit,
		Reset
	}

	public static partial class HtmlHelperExtensions
	{
		public static IHtmlContent Button(this IHtmlHelper helper, ButtonType buttonType, string buttonFace, object? htmlAttributes = null) {
			var button = new TagBuilder("button");

			button.MergeAttributes(htmlAttributes);
			button.MergeAttribute("type", buttonType.ToLower());
			button.InnerHtml.AppendHtml(buttonFace);

			return button;
		}
	}
}
