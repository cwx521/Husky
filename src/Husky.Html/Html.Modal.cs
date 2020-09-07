using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Husky.Html
{
	public enum ModalSize
	{
		[Label("", Description = "默认")]
		Default,
		[Label("modal-sm", Description = "小")]
		Small,
		[Label("modal-lg", Description = "大")]
		Large,
		[Label("modal-xl", Description = "超大")]
		ExtraLarge,
		[Label("modal-full", Description = "满屏")]
		Full
	}

	public static partial class HtmlHelperExtensions
	{
		public static IHtmlContent Modal(this IHtmlHelper helper, string id, string title, string loadContentFromUrl, ModalSize size = ModalSize.Default, bool showFooter = true) {
			var result = new HtmlContentBuilder();
			result.AppendHtml(BeginModal(id, title, size));
			result.AppendHtml($"<div data-load-mode='passive' data-load-from='{loadContentFromUrl}'></div>");
			result.AppendHtml(EndModal(showFooter));
			return result;
		}

		private static IHtmlContent ModalForConfirmation(this IHtmlHelper helper, string id, string message) {
			var result = new HtmlContentBuilder();
			result.AppendHtml(BeginModal(id, "确认", ModalSize.Default));
			result.AppendHtml($"<div class='mb-3'>{message}</div>");
			result.AppendHtml(EndModal(true, confirmButton: true));
			return result;
		}

		private static IHtmlContent BeginModal(string id, string title, ModalSize size = ModalSize.Default) {
			var result = new HtmlContentBuilder();
			result.AppendHtml($"<div id='{id}' class='modal fade'>")
				  .AppendHtml($"  <div class='modal-dialog {size.ToLabel()}'>")
				  .AppendHtml("     <div class='modal-content'>");

			if ( !string.IsNullOrEmpty(title) ) {
				result.AppendHtml("   <div class='modal-header'>")
					  .AppendHtml($"    <h5 class='modal-title'>{title}</h5>")
					  .AppendHtml("     <button type='button' class='close' data-dismiss='modal'>&times;</button>")
					  .AppendHtml("   </div>");
			}

			result.AppendHtml("       <div class='modal-body'>");
			return result;
		}

		private static IHtmlContent EndModal(bool showFooter, bool confirmButton = false) {
			var result = new HtmlContentBuilder();
			result.AppendHtml("       </div>"); //modal-body

			if ( showFooter ) {
				result.AppendHtml("   <div class='modal-footer'>");
				if ( confirmButton ) {
					result.AppendHtml(" <button type='button' class='btn btn-danger btn-confirm btn-submit' data-dismiss='modal'>确认</button>");
				}
				result.AppendHtml("     <button type='button' class='btn btn-secondary btn-close' data-dismiss='modal'>关闭</button>");
				result.AppendHtml("   </div>");
			}

			result.AppendHtml("     </div>");   //modal-content
			result.AppendHtml("   </div>");     //modal-dialog
			result.AppendHtml("</div>");        //modal
			return result;
		}
	}
}
