using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Husky.Html
{
	public static partial class HtmlHelperExtensions
	{
		public static IHtmlContent ButtonWithConfirm(this IHtmlHelper helper, string buttonFace, string message = "该操作不可恢复，确定要执行吗？", object? htmlAttributes = null) {
			var id = "_" + Crypto.RandomString();

			var button = new TagBuilder("button");
			button.MergeAttributes(htmlAttributes);
			button.MergeAttribute("type", "button");
			button.MergeAttribute("data-toggle", "modal");
			button.MergeAttribute("data-target", "#" + id);

			var result = new HtmlContentBuilder();
			result.AppendHtml(button.RenderStartTag());
			result.AppendHtml(buttonFace);
			result.AppendHtml(button.RenderEndTag());
			result.AppendHtml(helper.ModalForConfirmation(id, message));
			return result;
		}
	}
}
