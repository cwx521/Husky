using System;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Husky.Html.Bootstrap
{
	public static partial class HtmlHelperExtensions
	{
		public static IHtmlContent SwitchFor<TModel>(this IHtmlHelper<TModel> helper,
			Expression<Func<TModel, bool>> expression,
			string? label = null,
			string? additionalCssClass = null) {

			return helper.RenderBootstrapFormCheck(expression, FormCheckType.Switch, label, additionalCssClass);
		}
	}
}
