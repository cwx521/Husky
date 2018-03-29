using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Husky.Razor
{
	public enum ModalSize
	{
		[Label("")]
		Default,
		[Label("modal-sm")]
		Small,
		[Label("modal-lg")]
		Large,
		[Label("modal-xl")]
		ExtraLarge,
		[Label("modal-full")]
		Full
	}

	public static partial class HtmlHelperExtensions
	{
		public static IHtmlContent Modal(this IHtmlHelper helper, string id, string title, string loadContentFromUrl, ModalSize size = ModalSize.Default, bool showFooter = true) {
			var result = new HtmlContentBuilder();
			result.AppendHtml(helper.BeginModal(id, title, size));
			result.AppendHtml($"<div data-load-from='{loadContentFromUrl}'></div>");
			result.AppendHtml(helper.EndModal(showFooter));
			return result;
		}

		private static IHtmlContent ModalForConfirmation(this IHtmlHelper helper, string id, string message = null) {
			var result = new HtmlContentBuilder();
			result.AppendHtml(helper.BeginModal(id, "Confirmation", ModalSize.Default));
			result.AppendHtml($"<div class='mt-1 mb-3'>{message ?? "This action can not be restored, are you sure to proceed?"}</div>");
			result.AppendHtml(helper.EndModal(true, showConfirmButton: true));
			return result;
		}

		private static IHtmlContent BeginModal(this IHtmlHelper helper, string id, string title, ModalSize size = ModalSize.Default) {
			var result = new HtmlContentBuilder();
			result.AppendHtml($"<div class='modal fade' id='{id}' role='dialog'>");
			result.AppendHtml($"  <div class='modal-dialog {size.ToLabel()}' role='document'>");
			result.AppendHtml("     <div class='modal-content'>");

			if ( !string.IsNullOrEmpty(title) ) {
				result.AppendHtml("   <div class='modal-header'>");
				result.AppendHtml($"    <h5 class='modal-title'>{title}</h5>");
				result.AppendHtml("     <button type='button' class='close' data-dismiss='modal'>&times;</button>");
				result.AppendHtml("   </div>");
			}

			result.AppendHtml("       <div class='modal-body'>");
			return result;
		}

		private static IHtmlContent EndModal(this IHtmlHelper helper, bool showFooter, bool showConfirmButton = false) {
			var result = new HtmlContentBuilder();
			result.AppendHtml("       </div>");

			if ( showFooter ) {
				result.AppendHtml("   <div class='modal-footer'>");
				if ( showConfirmButton ) {
					result.AppendHtml(" <button type='button' class='btn btn-danger btn-confirm btn-submit' data-dismiss='modal'><i class='fa fa-check mr-1'></i>Confirm</button>");
				}
				result.AppendHtml("     <button type='button' class='btn btn-secondary btn-close' data-dismiss='modal'><i class='fa fa-ban mr-1'></i>关闭</button>");
				result.AppendHtml("   </div>");
			}

			result.AppendHtml("     </div>");   //modal-content
			result.AppendHtml("   </div>");     //modal-dialog
			result.AppendHtml("</div>");        //modal
			return result;
		}
	}
}
