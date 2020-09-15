using System;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Husky.Html
{
	public static partial class HtmlHelperExtensions
	{
		public static IHtmlContent CustomCheckBox(this IHtmlContent checkbox, string? label) {
			if ( checkbox is TagBuilder builder ) {
				return new HtmlString(PrettifyCustomControl(builder, CustomControlType.CheckBox, label));
			}
			throw new InvalidCastException($"The type of the parameter is not TagBuilder.");
		}
	}
}
