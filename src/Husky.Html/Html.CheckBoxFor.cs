using System;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Husky.Html
{
	public static partial class HtmlHelperExtensions
	{
		public static IHtmlContent CheckBoxFor<TModel>(this IHtmlHelper<TModel> helper,
			Expression<Func<TModel, bool>> expression,
			string? label = null,
			string? additionalCssClass = null) {

			return helper.RenderBootstrapCustomControl(expression, CustomControlType.CheckBox, label, additionalCssClass);
		}
	}
}
