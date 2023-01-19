using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Husky.Html.Bootstrap
{
	public static partial class HtmlHelperExtensions
	{
		public static IHtmlContent ModalFooter(this IHtmlHelper _,
			ButtonState confirmButton = ButtonState.NotHave,
			string confirmButtonFace = "确认",
			string confirmButtonCssClass = "btn-warning") {

			var result = new HtmlContentBuilder();

			result.AppendHtml("<div class='modal-footer inside-modal-body overflow-hidden d-flex'>");
			result.AppendHtml("	 <div class='align-self-center me-auto mr-auto'><span class='modal-footer-state'></span></div>");
			result.AppendHtml("	 <div class='align-self-center ms-auto ml-auto ps-3 pl-3'>");

			if (confirmButton != ButtonState.NotHave) {
				var button = new TagBuilder("button");

				button.Attributes.Add("type", "submit");
				button.Attributes.Add("class", $"btn {confirmButtonCssClass}");
				button.InnerHtml.AppendHtml(confirmButtonFace);

				if (confirmButton == ButtonState.Disabled) {
					button.Attributes.Add("disabled", "disabled");
				}
				else if (confirmButton == ButtonState.Hidden) {
					button.Attributes.Add("hidden", "hidden");
				}
				result.AppendHtml(button);
			}

			result.AppendHtml("<button type='button' class='btn btn-light border' data-dismiss='modal' data-bs-dismiss='modal'>关闭</button>");

			result.AppendHtml("	 </div>");
			result.AppendHtml("</div>");

			return result;
		}
	}
}