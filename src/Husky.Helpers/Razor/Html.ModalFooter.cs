using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Husky.Razor
{
	public enum SaveButtonState
	{
		NotHave,
		Enabled,
		Disabled,
		Hidden
	}

	public static partial class HtmlHelperExtensions
	{
		public static IHtmlContent ModalFooter(this IHtmlHelper helper, SaveButtonState saveButton = SaveButtonState.NotHave) {
			var result = new HtmlContentBuilder();

			result.AppendHtml("<div class='modal-footer overflow-hidden'>");
			result.AppendHtml("	<div class='modal-footer-button-container'>");

			if ( saveButton != SaveButtonState.NotHave ) {
				result.AppendHtml(CreateSaveButton(saveButton));
			}
			result.AppendHtml("<button type='button' class='btn btn-lg btn-secondary btn-close' data-dismiss='modal'>关闭</button>");

			result.AppendHtml("	</div>");
			result.AppendHtml("	<div class='modal-footer-state-container'><span class='modal-footer-state'></span></div>");
			result.AppendHtml("</div>");

			return result;
		}

		private static IHtmlContent CreateSaveButton(SaveButtonState buttonState) {
			if ( buttonState == SaveButtonState.NotHave ) {
				return null;
			}

			var tag = new TagBuilder("button");

			tag.Attributes.Add("type", "submit");
			tag.Attributes.Add("class", "btn btn-lg btn-primary");
			tag.InnerHtml.AppendHtml("确认");

			if ( buttonState == SaveButtonState.Disabled ) {
				tag.Attributes.Add("disabled", "disabled");
			}
			else if ( buttonState == SaveButtonState.Hidden ) {
				tag.Attributes.Add("hidden", "hidden");
			}
			return tag;
		}
	}
}