using System.Text;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Husky.Razor
{
	public static partial class HtmlHelperExtensions
	{
		public static IHtmlContent BreadCrumb(this IHtmlHelper helper, params string[] items) {
			var sb = new StringBuilder();

			sb.AppendLine("<nav aria-label='breadcrumb' role='navigation'>");
			sb.AppendLine("  <ol class='breadcrumb'>");
			sb.AppendLine("    <li class='breadcrumb-item'><a href='/'><i class='fa fa-home mr-1'></i>Home</a></li>");

			for ( int i = 0; i < items.Length; i++ ) {
				if ( string.IsNullOrEmpty(items[i]) ) {
					continue;
				}
				sb.AppendLine($"<li class='breadcrumb-item'>{items[i]}</li>");
			}

			//sb.AppendLine("<li class='ml-auto'><a href='javascript:history.back()'><i class='fa fa-chevron-circle-left mr-1'></i>Back</a></li>");

			sb.AppendLine("  </ol>");
			sb.AppendLine("</nav>");

			return new HtmlString(sb.ToString());
		}
	}
}
