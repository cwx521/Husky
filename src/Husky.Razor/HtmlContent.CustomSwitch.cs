using System;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Husky.Razor
{
	public static partial class HtmlHelperExtensions
	{
		public static IHtmlContent CustomSwitch(this IHtmlContent checkbox, string label) {
			if ( checkbox is TagBuilder builder ) {
				return new HtmlString(PrettifyCustomControl(builder, CustomControlType.Switch, label));
			}
			throw new InvalidCastException($"The type of the parameter is not TagBuilder.");
		}
	}
}
