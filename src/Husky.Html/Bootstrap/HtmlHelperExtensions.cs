using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Husky.Html.Bootstrap
{
	public static partial class HtmlHelperExtensions
	{
		private static string ToHtml(this IHtmlContent htmlContent) {
			using var writer = new StringWriter();

			htmlContent.WriteTo(writer, HtmlEncoder.Default);
			return writer.ToString();
		}

		private static TagBuilder MergeAttributes(this TagBuilder tagBuilder, object? htmlAttributes) {
			if (htmlAttributes != null) {
				var props = htmlAttributes.GetType().GetProperties();
				foreach (var p in props) {
					tagBuilder.MergeAttribute(p.Name, p.GetValue(htmlAttributes) as string, true);
				}
			}
			return tagBuilder;
		}

		private static string BootstrapFormCheck(TagBuilder inputTag, FormCheckType formCheckType, string? label, string? additionalCssClass = null) {
			if (!inputTag.Attributes.TryGetValue("class", out var cssClass) || !cssClass!.Contains("form-check-input")) {
				inputTag.AddCssClass("form-check-input");
				inputTag.AddCssClass("custom-control-input");
			}
			return $@"<div class='form-check {(formCheckType == FormCheckType.Switch ? "form-switch" : "")} custom-control custom-{formCheckType.ToLower()} {additionalCssClass}'>
				{inputTag.ToHtml()}
				<label class='form-check-label custom-control-label' for='{inputTag.Attributes.GetValueOrDefault("id")}'>{label}</span>
			</div>";
		}

		private static IHtmlContent RenderBootstrapFormCheck<TModel>(this IHtmlHelper<TModel> helper,
			Expression<Func<TModel, bool>> expression,
			FormCheckType formCheckType = FormCheckType.CheckBox,
			string? label = null,
			string? additionalCssClass = null) {

			var inputTag = new TagBuilder("input") {
				TagRenderMode = TagRenderMode.SelfClosing
			};

			inputTag.Attributes.Add("id", helper.IdFor(expression));
			inputTag.Attributes.Add("name", helper.NameFor(expression));
			inputTag.Attributes.Add("type", "checkbox");
			inputTag.Attributes.Add("value", "true");

			if (helper.ViewData.Model != null) {
				try {
					var value = expression.Compile().Invoke(helper.ViewData.Model);
					if (value) {
						inputTag.Attributes.Add("checked", "checked");
					}
				}
				catch (NullReferenceException) { }
				catch { throw; }
			}

			var formCheck = BootstrapFormCheck(inputTag, formCheckType, label ?? helper.DisplayNameFor(expression), additionalCssClass);
			return new HtmlString(formCheck);
		}

		private static IHtmlContent RenderBootstrapFormCheckGroup<TModel, TResult>(this IHtmlHelper<TModel> helper,
			Expression<Func<TModel, TResult>> expression,
			FormCheckType formCheckType,
			IEnumerable<SelectListItem> selectListItems,
			LayoutDirection layoutDirection = LayoutDirection.Horizontal,
			object? htmlAttributes = null) {

			var result = new HtmlContentBuilder();

			foreach (var item in selectListItems) {
				var inputTag = new TagBuilder("input") {
					TagRenderMode = TagRenderMode.SelfClosing
				};

				inputTag.Attributes.Add("id", "_" + Crypto.RandomString());
				inputTag.Attributes.Add("name", helper.NameFor(expression));
				inputTag.Attributes.Add("value", item.Value);
				inputTag.Attributes.Add("type", formCheckType == FormCheckType.Switch ? "checkbox" : formCheckType.ToLower());
				if (item.Selected) {
					inputTag.Attributes.Add("checked", "checked");
				}
				inputTag.MergeAttributes(htmlAttributes);

				var additionalCssClass = (layoutDirection == LayoutDirection.Horizontal ? "form-check-inline custom-control-inline" : "");
				var formCheck = BootstrapFormCheck(inputTag, formCheckType, item.Text, additionalCssClass);

				result.AppendHtml(formCheck);
			}
			return result;
		}
	}
}
