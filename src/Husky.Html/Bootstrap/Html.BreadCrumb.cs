using System.Text;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Husky.Html.Bootstrap
{
	public static partial class HtmlHelperExtensions
	{
		public static IHtmlContent BreadCrumb(this IHtmlHelper _, params string[] items) {
			var sb = new StringBuilder();

			sb.AppendLine("<nav>");
			sb.AppendLine("  <ol class='breadcrumb'>");
			sb.AppendLine("    <li class='breadcrumb-item breadcrumb-item-home'><a href='/'><i class='fa fa-home'></i></a></li>");

			foreach (var item in items) {
				if (!string.IsNullOrEmpty(item)) {
					sb.AppendLine($"<li class='breadcrumb-item'>{item}</li>");
				}
			}

			sb.AppendLine("  </ol>");
			sb.AppendLine("</nav>");

			return new HtmlString(sb.ToString());
		}
	}
}
