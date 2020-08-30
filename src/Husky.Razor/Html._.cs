using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Husky.Razor
{
	public static partial class HtmlHelperExtensions
	{
		private static string ToHtml(this IHtmlContent tag) {
			using var writer = new StringWriter();

			tag.WriteTo(writer, HtmlEncoder.Default);
			return writer.ToString() ?? string.Empty;
		}

		private static TagBuilder MergeAttributes(this TagBuilder tagBuilder, object htmlAttributes) {
			if ( htmlAttributes != null ) {
				var props = htmlAttributes.GetType().GetProperties();
				foreach ( var p in props ) {
					tagBuilder.MergeAttribute(p.Name, p.GetValue(htmlAttributes) as string, true);
				}
			}
			return tagBuilder;
		}

		private static void PrettifyFormControl(this TagBuilder tagBuilder) {
			if ( tagBuilder.Attributes.TryGetValue("class", out var cssClass) && cssClass.Contains("form-control") ) {
				return;
			}
			if ( tagBuilder.Attributes.TryGetValue("type", out var typeName) && new[] { "checkbox", "radio", "button", "submit", "reset" }.Contains(typeName) ) {
				return;
			}
			if ( new[] { "input", "select", "textarea" }.Contains(tagBuilder.TagName) ) {
				tagBuilder.AddCssClass("form-control");
			}
		}

		private enum CustomControlType
		{
			CheckBox,
			Radio,
			Switch
		}

		private static string PrettifyCustomControl(TagBuilder inputTag, CustomControlType customControlType, string label, string addtionalCssClass = null) {
			if ( !inputTag.Attributes.TryGetValue("class", out var cssClass) || !cssClass.Contains("custom-control-input") ) {
				inputTag.AddCssClass("custom-control-input");
			}
			return $@"<div class='custom-control custom-{customControlType.ToLower()} {addtionalCssClass}'>
				{inputTag.ToHtml()}
				<label class='custom-control-label' for='{inputTag.Attributes.GetValueOrDefault("id")}'>{label}</span>
			</div>";
		}

		private static IHtmlContent RenderCustomControlGroupFor<TModel, TResult>(this IHtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression, CustomControlType customControlType, IEnumerable<SelectListItem> selectListItems, LayoutDirection layoutDirection = LayoutDirection.Horizontal, object htmlAttributes = null) {
			var result = new HtmlContentBuilder();

			foreach ( var item in selectListItems ) {
				var inputTag = new TagBuilder("input") {
					TagRenderMode = TagRenderMode.SelfClosing
				};

				inputTag.AddCssClass("custom-control-input");
				inputTag.Attributes.Add("type", customControlType == CustomControlType.Switch ? "checkbox" : customControlType.ToLower());
				inputTag.Attributes.Add("id", "_" + Crypto.RandomString());
				inputTag.Attributes.Add("name", helper.NameFor(expression));
				inputTag.Attributes.Add("value", item.Value);
				if ( item.Selected ) {
					inputTag.Attributes.Add("checked", "checked");
				}
				inputTag.MergeAttributes(htmlAttributes);

				// output custom-checkbox (bootstrap)
				var outerDivCssClass = (layoutDirection == LayoutDirection.Horizontal ? "custom-control-inline custom-control-group-item" : "custom-control-group-item");
				var customControl = PrettifyCustomControl(inputTag, customControlType, item.Text, outerDivCssClass);

				result.AppendHtml(customControl);
			}
			return result;
		}
	}
}
