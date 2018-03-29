using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Husky.Razor
{
	public enum ButtonType
	{
		Button,
		Submit,
		Reset
	}

	public static partial class HtmlHelperExtensions
	{
		public static IHtmlContent Button(this IHtmlHelper helper, ButtonType type, string buttonFace, object htmlAttributes = null) {
			var button = new TagBuilder("button");
			button.Attributes.Add("type", type.ToLower());

			if ( htmlAttributes != null ) {
				var props = htmlAttributes.GetType().GetProperties();
				foreach ( var p in props ) {
					button.MergeAttribute(p.Name, p.GetValue(htmlAttributes) as string, true);
				}
			}
			button.InnerHtml.AppendHtml(buttonFace);
			return button;
		}
	}
}
