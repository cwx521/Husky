using System;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Husky.Html
{
	public static partial class HtmlHelperExtensions
	{
		public static IHtmlContent Prop(this IHtmlContent tagBuilder, string propName, object propValue) {
			if ( tagBuilder == null ) {
				return null;
			}
			if ( tagBuilder is TagBuilder ctl ) {
				ctl.PrettifyFormControl();

				if ( propValue == null ) {
					ctl.Attributes.Remove(propName);
				}
				else {
					ctl.MergeAttribute(propName, propValue?.ToString());
				}
				return ctl;
			}
			throw new InvalidCastException($"The type of the parameter is not TagBuilder.");
		}
	}
}
