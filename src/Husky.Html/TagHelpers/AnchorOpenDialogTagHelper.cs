using System;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Husky.Html
{
	[HtmlTargetElement("a", Attributes = IdentifierAttributeName)]
	[HtmlTargetElement("a", Attributes = ModalTitleAttributeName)]
	public class AnchorOpenModalTagHelper : TagHelper
	{
		public AnchorOpenModalTagHelper() {
		}

		public const string IdentifierAttributeName = "open-modal";
		public const string ModalTitleAttributeName = "modal-title";

		[HtmlAttributeName(IdentifierAttributeName)]
		public bool OpenDialog { get; set; } = true;

		[HtmlAttributeName(ModalTitleAttributeName)]
		public string? ModalTitle { get; set; }

		[HtmlAttributeName("target")]
		public string Target { get; set; } = "#modal";

		[HtmlAttributeName("modal-size")]
		public string ModalSize { get; set; } = "modal-md";

		[HtmlAttributeName("modal-footer")]
		public bool ModalFooter { get; set; }

		public override void Process(TagHelperContext context, TagHelperOutput output) {
			if ( OpenDialog == false ) {
				return;
			}

			output.Attributes.SetAttribute("data-ajax", "husky");
			output.Attributes.SetAttribute("data-toggle", "modal");

			output.Attributes.SetAttribute("data-target", Target);

			if ( !ModalSize.Equals("modal-md", StringComparison.OrdinalIgnoreCase) ) {
				output.Attributes.SetAttribute("data-modal-size", ModalSize);
			}
			if ( !string.IsNullOrEmpty(ModalTitle) ) {
				output.Attributes.SetAttribute("data-modal-title", ModalTitle);
			}
			if ( ModalFooter ) {
				output.Attributes.SetAttribute("data-modal-footer", "true");
			}

			output.Attributes.RemoveAll(IdentifierAttributeName);
		}
	}
}
