using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Husky.Razor
{
	public static partial class HtmlHelperExtensions
	{
		public static IHtmlContent CheckBoxListFor<TModel, TResult>(this IHtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression, IEnumerable<SelectListItem> selectListItems, LayoutDirection layoutDirection = LayoutDirection.Horizontal, object htmlAttributes = null) where TResult : IEnumerable {
			if ( helper.ViewData.Model != null ) {
				try {
					// in case when expression = {x.Aaa.Bbb}, even x is not null,x.Aaa can be null
					// turns out fail to get the value by invoking the compiled expression
					// any better idea than a try catch?
					var value = expression.Compile().Invoke(helper.ViewData.Model);
					foreach ( var i in value ) {
						selectListItems.Where(x => x.Value == i?.ToString()).AsParallel().ForAll(x => x.Selected = true);
					}
				}
				catch ( NullReferenceException ) { /* ignore NullReferenceException */ }
				catch { throw; }
			}
			return helper.RenderCheckBoxOrRadioButtonListFor(expression, BoxType.CheckBox, selectListItems, layoutDirection, htmlAttributes);
		}

		public static IHtmlContent CheckBoxListFor<TModel, TResult>(this IHtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression, Type enumType, LayoutDirection layoutDirection = LayoutDirection.Horizontal, object htmlAttributes = null) where TResult : IEnumerable {
			if ( enumType == null ) {
				throw new ArgumentNullException(nameof(enumType));
			}
			var selectListItems = helper.GetEnumSelectList(enumType);

			if ( helper.ViewData.Model != null ) {
				try {
					var value = expression.Compile().Invoke(helper.ViewData.Model);
					foreach ( var i in value ) {
						var str = i?.ToString();
						if ( Enum.TryParse(enumType, str, out var enumVal) ) {
							str = ((int)enumVal).ToString();
						}
						selectListItems.Where(x => x.Value == str).AsParallel().ForAll(x => x.Selected = true);
					}
				}
				catch ( NullReferenceException ) { }
				catch { throw; }
			}
			return helper.RenderCheckBoxOrRadioButtonListFor(expression, BoxType.CheckBox, selectListItems, layoutDirection, htmlAttributes);
		}
	}
}