using System;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.Principal
{
	public class MustLoginFilter : IPageFilter, IActionFilter
	{
		public void OnPageHandlerExecuting(PageHandlerExecutingContext context) {
			if (context.ActionDescriptor.ModelTypeInfo?.IsDefined(typeof(AllowAnonymousAttribute)) ?? false) {
				return;
			}

			var principal = context.HttpContext.RequestServices.GetRequiredService<IPrincipalUser>();
			var appSettings = context.HttpContext.RequestServices.GetService<IConfiguration>();
			if (principal.IsAnonymous) {
				var baseUrl = context.HttpContext.Request.ProtocolAndHost();
				var loginRoute = appSettings?.GetValue<string>("Login") ?? "/login";
				var stepUrl = baseUrl + loginRoute + "?redir=" + context.HttpContext.Request.Url();
				SafeRedirect(context, stepUrl);
			}
		}

		public void OnActionExecuting(ActionExecutingContext context) {
			if ((context.ActionDescriptor as ControllerActionDescriptor)?.MethodInfo?.IsDefined(typeof(AllowAnonymousAttribute)) ?? false) {
				return;
			}

			var principal = context.HttpContext.RequestServices.GetRequiredService<IPrincipalUser>();
			if (principal.IsAnonymous) {
				context.Result = new Failure("需要先登录");
			}
		}


		public void OnPageHandlerSelected(PageHandlerSelectedContext context) { }
		public void OnPageHandlerExecuted(PageHandlerExecutedContext context) { }
		public void OnActionExecuted(ActionExecutedContext context) { }


		void SafeRedirect(PageHandlerExecutingContext context, string url) {
			if (!context.ActionDescriptor.DisplayName?.Equals(url, StringComparison.OrdinalIgnoreCase) ?? true) {
				context.Result = new RedirectResult(url);
			}
		}
	}
}