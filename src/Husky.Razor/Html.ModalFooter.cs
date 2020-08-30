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
		public static IHtmlContent ModalFooter(this IHtmlHelper helper, SaveButtonState saveButton = SaveButtonState.NotHave, string saveButtonText = "确认", string buttonScheme = "btn-warning") {
			var result = new HtmlContentBuilder();

			result.AppendHtml("<div class='modal-footer inside-modal-body overflow-hidden d-flex'>");
			result.AppendHtml("	 <div class='align-self-center'><span class='modal-footer-state'></span></div>");
			result.AppendHtml("	 <div class='align-self-center pl-3 ml-auto'>");

			if ( saveButton != SaveButtonState.NotHave ) {
				var tag = new TagBuilder("button");

				tag.Attributes.Add("type", "submit");
				tag.Attributes.Add("class", $"btn btn-lg {buttonScheme}");
				tag.InnerHtml.AppendHtml(saveButtonText);

				if ( saveButton == SaveButtonState.Disabled ) {
					tag.Attributes.Add("disabled", "disabled");
				}
				else if ( saveButton == SaveButtonState.Hidden ) {
					tag.Attributes.Add("hidden", "hidden");
				}
				result.AppendHtml(tag);
			}

			result.AppendHtml("<button type='button' class='btn btn-light border btn-lg btn-close' data-dismiss='modal'>关闭</button>");

			result.AppendHtml("	 </div>");
			result.AppendHtml("</div>");

			return result;
		}
	}
}