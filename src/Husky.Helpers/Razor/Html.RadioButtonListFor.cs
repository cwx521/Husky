using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Husky.Razor
{
	public static partial class HtmlHelperExtensions
	{
		public static IHtmlContent RadioButtonListFor<TModel, TResult>(this IHtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression, IEnumerable<SelectListItem> selectListItems, LayoutDirection layoutDirection = LayoutDirection.Horizontal, object htmlAttributes = null) {
			if ( helper.ViewData.Model != null ) {
				try {
					// when expression = {x.Aaa.Bbb}, NullReferenceException can happen
					var value = expression.Compile().Invoke(helper.ViewData.Model);
					selectListItems.Where(x => x.Value == value?.ToString()).AsParallel().ForAll(x => x.Selected = true);
				}
				catch ( NullReferenceException ) { }
				catch { throw; }
			}
			return helper.RenderCheckBoxOrRadioButtonListFor(expression, BoxType.RadioButton, selectListItems, layoutDirection, htmlAttributes);
		}

		public static IHtmlContent RadioButtonListFor<TModel, TResult>(this IHtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression, Type enumType, LayoutDirection layoutDirection = LayoutDirection.Horizontal, object htmlAttributes = null) {
			if ( enumType == null ) {
				throw new ArgumentNullException(nameof(enumType));
			}

			var selectListItems = helper.GetEnumSelectList(enumType);

			//hack the specific enum type 'YesNo', want to put Yes before No. 
			if ( enumType == typeof(YesNo) ) {
				selectListItems = selectListItems.Reverse();
			}

			if ( helper.ViewData.Model != null ) {
				try {
					var value = expression.Compile().Invoke(helper.ViewData.Model);
					var str = value?.ToString();
					if ( Enum.TryParse(enumType, str, out var enumVal) ) {
						str = ((int)enumVal).ToString();
					}
					selectListItems.Where(x => x.Value == str).AsParallel().ForAll(x => x.Selected = true);
				}
				catch ( NullReferenceException ) { }
				catch { throw; }
			}
			return helper.RenderCheckBoxOrRadioButtonListFor(expression, BoxType.RadioButton, selectListItems, layoutDirection, htmlAttributes);
		}
	}
}