using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Husky.Html.Bootstrap
{
	public static partial class HtmlHelperExtensions
	{
		public static IHtmlContent CustomSwitchListFor<TModel, TResult>(this IHtmlHelper<TModel> helper,
			Expression<Func<TModel, TResult>> expression,
			IEnumerable<SelectListItem> selectListItems,
			LayoutDirection layoutDirection = LayoutDirection.Horizontal,
			object? htmlAttributes = null)
			where TResult : IEnumerable {

			if ( helper.ViewData.Model != null ) {
				try {
					var value = expression.Compile().Invoke(helper.ViewData.Model);
					foreach ( var i in value ) {
						selectListItems.Where(x => x.Value == i?.ToString()).AsParallel().ForAll(x => x.Selected = true);
					}
				}
				catch ( NullReferenceException ) { }
				catch { throw; }
			}
			return helper.RenderBootstrapCustomControlGroup(expression, CustomControlType.Switch, selectListItems, layoutDirection, htmlAttributes);
		}

		public static IHtmlContent CustomSwitchListFor<TModel, TResult>(this IHtmlHelper<TModel> helper,
			Expression<Func<TModel, TResult>> expression,
			Type enumType,
			LayoutDirection layoutDirection = LayoutDirection.Horizontal,
			object? htmlAttributes = null)
			where TResult : IEnumerable {

			if ( enumType == null ) {
				throw new ArgumentNullException(nameof(enumType));
			}
			var selectListItems = EnumHelper.ToSelectListItems(enumType, useIntValue: false);
			return helper.CustomSwitchListFor(expression, selectListItems, layoutDirection, htmlAttributes);
		}
	}
}