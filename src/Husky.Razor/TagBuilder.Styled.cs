using System;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Husky.Razor
{
	public static partial class HtmlHelperExtensions
	{
		public static IHtmlContent Styled(this IHtmlContent tagBuilder) {
			if ( tagBuilder == null ) {
				return null;
			}
			if ( tagBuilder is TagBuilder ctl ) {
				ctl.BeautifyTextBoxOrDropDown();
				return ctl;
			}
			throw new InvalidCastException($"The type of the parameter is not TagBuilder.");
		}
	}
}
