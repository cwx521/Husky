using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Husky.Html
{
	public static partial class HtmlHelperExtensions
	{
		private static string ToHtml(this IHtmlContent htmlContent) {
			using var writer = new StringWriter();

			htmlContent.WriteTo(writer, HtmlEncoder.Default);
			return writer.ToString();
		}

		private static TagBuilder MergeAttributes(this TagBuilder tagBuilder, object? htmlAttributes) {
			if ( htmlAttributes != null ) {
				var props = htmlAttributes.GetType().GetProperties();
				foreach ( var p in props ) {
					tagBuilder.MergeAttribute(p.Name, p.GetValue(htmlAttributes) as string, true);
				}
			}
			return tagBuilder;
		}

		private static string BootstrapCustomControl(TagBuilder inputTag, CustomControlType customControlType, string? label, string? additionalCssClass = null) {
			if ( !inputTag.Attributes.TryGetValue("class", out var cssClass) || !cssClass.Contains("custom-control-input") ) {
				inputTag.AddCssClass("custom-control-input");
			}
			return $@"<div class='custom-control custom-{customControlType.ToLower()} {additionalCssClass}'>
				{inputTag.ToHtml()}
				<label class='custom-control-label' for='{inputTag.Attributes.GetValueOrDefault("id")}'>{label}</span>
			</div>";
		}

		private static IHtmlContent RenderBootstrapCustomControl<TModel>(this IHtmlHelper<TModel> helper,
			Expression<Func<TModel, bool>> expression,
			CustomControlType customControlType = CustomControlType.CheckBox,
			string? label = null,
			string? additionalCssClass = null) {

			var inputTag = new TagBuilder("input") {
				TagRenderMode = TagRenderMode.SelfClosing
			};

			inputTag.AddCssClass("custom-control-input");
			inputTag.Attributes.Add("id", helper.IdFor(expression));
			inputTag.Attributes.Add("name", helper.NameFor(expression));
			inputTag.Attributes.Add("type", "checkbox");
			inputTag.Attributes.Add("value", "true");

			if ( helper.ViewData.Model != null ) {
				try {
					var value = expression.Compile().Invoke(helper.ViewData.Model);
					if ( value ) {
						inputTag.Attributes.Add("checked", "checked");
					}
				}
				catch ( NullReferenceException ) { }
				catch { throw; }
			}

			var customControl = BootstrapCustomControl(inputTag, customControlType, label ?? helper.DisplayNameFor(expression), additionalCssClass);
			return new HtmlString(customControl);
		}

		private static IHtmlContent RenderBootstrapCustomControlGroup<TModel, TResult>(this IHtmlHelper<TModel> helper,
			Expression<Func<TModel, TResult>> expression,
			CustomControlType customControlType,
			IEnumerable<SelectListItem> selectListItems,
			LayoutDirection layoutDirection = LayoutDirection.Horizontal,
			object? htmlAttributes = null) {

			var result = new HtmlContentBuilder();

			foreach ( var item in selectListItems ) {
				var inputTag = new TagBuilder("input") {
					TagRenderMode = TagRenderMode.SelfClosing
				};

				inputTag.AddCssClass("custom-control-input");
				inputTag.Attributes.Add("id", "_" + Crypto.RandomString());
				inputTag.Attributes.Add("name", helper.NameFor(expression));
				inputTag.Attributes.Add("value", item.Value);
				inputTag.Attributes.Add("type", customControlType == CustomControlType.Switch ? "checkbox" : customControlType.ToLower());
				if ( item.Selected ) {
					inputTag.Attributes.Add("checked", "checked");
				}
				inputTag.MergeAttributes(htmlAttributes);

				// output div.custom-control (bootstrap)
				// custom-control-list-item is an additional cssclass which is just defined at this place, not from bootstrap
				var additionalCssClass = (layoutDirection == LayoutDirection.Horizontal ? "custom-control-inline custom-control-list-item" : "custom-control-list-item");
				var customControl = BootstrapCustomControl(inputTag, customControlType, item.Text, additionalCssClass);

				result.AppendHtml(customControl);
			}
			return result;
		}
	}
}
