using System;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Husky.Html
{
	public static partial class HtmlHelperExtensions
	{
		public static IHtmlContent AddCssClass(this IHtmlContent tagBuilder, string cssClass) {
			if ( tagBuilder is TagBuilder ctl ) {
				ctl.PrettifyFormControl();
				ctl.AddCssClass(cssClass);
				return ctl;
			}
			throw new InvalidCastException($"The type of the parameter is not TagBuilder.");
		}
	}
}
