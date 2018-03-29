using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Husky.Razor
{
	public static partial class HtmlHelperExtensions
	{
		public static IHtmlContent ButtonWithConfirm(this IHtmlHelper helper, string buttonFace, string confirmMessage = null, object htmlAttributes = null) {
			var id = Crypto.RandomString();

			var button = new TagBuilder("button");
			button.Attributes.Add("type", "button");

			if ( htmlAttributes != null ) {
				var props = htmlAttributes.GetType().GetProperties();
				foreach ( var p in props ) {
					button.MergeAttribute(p.Name, p.GetValue(htmlAttributes) as string, true);
				}
			}
			button.MergeAttribute("data-toggle", "modal");
			button.MergeAttribute("data-target", "#" + id);

			var result = new HtmlContentBuilder();
			result.AppendHtml(button.RenderStartTag());
			result.AppendHtml(buttonFace);
			result.AppendHtml(button.RenderEndTag());
			result.AppendHtml(helper.ModalForConfirmation(id, confirmMessage));
			return result;
		}
	}
}
