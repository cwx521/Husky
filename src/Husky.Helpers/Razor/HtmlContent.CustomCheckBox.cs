using System;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Husky.Razor
{
	public static partial class HtmlHelperExtensions
	{
		public static IHtmlContent CustomCheckBox(this IHtmlContent checkbox, string label) {
			if ( checkbox is TagBuilder tb ) {
				tb.AddCssClass("custom-control-input");
				return new HtmlString(BeautifyCheckBoxOrRadioButton(tb.ToHtml(), label));
			}
			throw new InvalidCastException($"The type of the parameter is not TagBuilder.");
		}
	}
}
