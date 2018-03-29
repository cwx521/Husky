using System;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Husky.Razor
{
	public static partial class HtmlHelperExtensions
	{
		public static IHtmlContent CustomRadioButton(this IHtmlContent radioButton, string label) {
			if ( radioButton is TagBuilder tb ) {
				tb.AddCssClass("custom-control-input");
				return new HtmlString(BeautifyCheckBoxOrRadioButton(tb.ToHtml(), label));
			}
			throw new InvalidCastException($"The type of the parameter is not TagBuilder.");
		}
	}
}
