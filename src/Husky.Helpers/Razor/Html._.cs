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
			using ( var writer = new StringWriter() ) {
				tag.WriteTo(writer, HtmlEncoder.Default);
				return writer.ToString() ?? string.Empty;
			}
		}

		private static string BeautifyCheckBoxOrRadioButton(string html, string label, string addtionalCssClass = null) {
			return html == null ? null :
				$@"<label class='custom-control custom-{(html.Contains("checkbox") ? "checkbox" : "radio")} {addtionalCssClass}'>
					{html}
					<span class='custom-control-indicator'></span>
					<span class='custom-control-description'>{label}</span>
				</label>";
		}

		private static void BeautifyTextBoxOrDropDown(this TagBuilder tagBuilder) {
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

		private enum BoxType
		{
			[Label("checkbox")]
			CheckBox,
			[Label("radio")]
			RadioButton
		}

		private static IHtmlContent RenderCheckBoxOrRadioButtonListFor<TModel, TResult>(this IHtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression, BoxType boxType, IEnumerable<SelectListItem> selectListItems, LayoutDirection layoutDirection = LayoutDirection.Horizontal, object htmlAttributes = null) {
			var result = new HtmlContentBuilder();

			foreach ( var item in selectListItems ) {
				// build <input type='checkbox|radio' />
				var inputTag = new TagBuilder("input") {
					TagRenderMode = TagRenderMode.SelfClosing
				};
				inputTag.AddCssClass("custom-control-input");
				inputTag.Attributes.Add("type", boxType.ToLabel());
				inputTag.Attributes.Add("name", expression.Body.ToString().Right("."));
				inputTag.Attributes.Add("value", item.Value);
				if ( item.Selected ) {
					inputTag.Attributes.Add("checked", "checked");
				}
				if ( htmlAttributes != null ) {
					var props = htmlAttributes.GetType().GetProperties();
					foreach ( var p in props ) {
						inputTag.MergeAttribute(p.Name, p.GetValue(htmlAttributes) as string, true);
					}
				}

				// as custom-checkbox (bootstrap)
				var str = BeautifyCheckBoxOrRadioButton(inputTag.ToHtml(), item.Text.JudgeWords(), addtionalCssClass: "list-box-item");
				result.AppendHtml(layoutDirection == LayoutDirection.Vertical ? $"<div>{str}</div>" : str);
			}
			return result;
		}
	}
}
