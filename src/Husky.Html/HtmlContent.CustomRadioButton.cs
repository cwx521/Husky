using System;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Husky.Html
{
	public static partial class HtmlHelperExtensions
	{
		public static IHtmlContent CustomRadioButton(this IHtmlContent radioButton, string? label) {
			if ( radioButton is TagBuilder builder ) {
				return new HtmlString(PrettifyCustomControl(builder, CustomControlType.Radio, label));
			}
			throw new InvalidCastException($"The type of the parameter is not TagBuilder.");
		}
	}
}
