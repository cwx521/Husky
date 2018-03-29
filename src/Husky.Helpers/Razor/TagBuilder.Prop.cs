using System;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Husky.Razor
{
	public static partial class HtmlHelperExtensions
	{
		public static IHtmlContent Prop(this IHtmlContent tagBuilder, string tagPropName, object tagPropValue) {
			if ( tagBuilder == null ) {
				return null;
			}
			if ( tagBuilder is TagBuilder ctl ) {
				ctl.BeautifyTextBoxOrDropDown();

				if ( tagPropValue == null ) {
					ctl.Attributes.Remove(tagPropName);
				}
				else {
					ctl.MergeAttribute(tagPropName, tagPropValue?.ToString());
				}
				return ctl;
			}
			throw new InvalidCastException($"The type of the parameter is not TagBuilder.");
		}
	}
}
