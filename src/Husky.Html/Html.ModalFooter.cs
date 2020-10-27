using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Husky.Html
{
	public static partial class HtmlHelperExtensions
	{
		public static IHtmlContent ModalFooter(this IHtmlHelper helper,
			ButtonState confirmationButton = ButtonState.NotHave,
			string confirmationButtonFace = "确认",
			string confirmationButtonScheme = "btn-warning") {

			var result = new HtmlContentBuilder();

			result.AppendHtml("<div class='modal-footer inside-modal-body overflow-hidden d-flex'>");
			result.AppendHtml("	 <div class='align-self-center'><span class='modal-footer-state'></span></div>");
			result.AppendHtml("	 <div class='align-self-center ml-auto pl-3'>");

			if ( confirmationButton != ButtonState.NotHave ) {
				var button = new TagBuilder("button");

				button.Attributes.Add("type", "submit");
				button.Attributes.Add("class", $"btn btn-lg {confirmationButtonScheme}");
				button.InnerHtml.AppendHtml(confirmationButtonFace);

				if ( confirmationButton == ButtonState.Disabled ) {
					button.Attributes.Add("disabled", "disabled");
				}
				else if ( confirmationButton == ButtonState.Hidden ) {
					button.Attributes.Add("hidden", "hidden");
				}
				result.AppendHtml(button);
			}

			result.AppendHtml("<button type='button' class='btn btn-light border btn-lg btn-close' data-dismiss='modal'>关闭</button>");

			result.AppendHtml("	 </div>");
			result.AppendHtml("</div>");

			return result;
		}
	}
}