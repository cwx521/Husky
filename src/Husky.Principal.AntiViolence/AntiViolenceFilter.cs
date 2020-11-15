using System;
using Husky.KeyValues;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.Principal.AntiViolence
{
	public class AntiViolenceFilter : IPageFilter, IActionFilter
	{
		public void OnPageHandlerSelected(PageHandlerSelectedContext context) {
		}

		public void OnPageHandlerExecuted(PageHandlerExecutedContext context) {
		}

		public void OnPageHandlerExecuting(PageHandlerExecutingContext context) {
			OnExecuting(context);
		}

		public void OnActionExecuted(ActionExecutedContext context) {
		}

		public void OnActionExecuting(ActionExecutingContext context) {
			OnExecuting(context);
		}

		private static void OnExecuting(FilterContext context) {
			if ( context.HttpContext.Request.Method == "GET" ) {
				return;
			}

			var keyValueManager = context.HttpContext.RequestServices.GetService<IKeyValueManager>();
			var ms = keyValueManager?.HttpPostsMinimumIntervalMilliseconds() ?? 300;
			var principal = context.HttpContext.RequestServices.GetRequiredService<IPrincipalUser>();
			var blocker = new AntiViolenceDefender(principal);

			if ( blocker.GetTimer().AddMilliseconds(ms) < DateTime.Now ) {
				blocker.SetTimer();
			}
			else {
				blocker.SetTimer(DateTime.Now.AddMilliseconds(ms));
				context.ModelState.AddModelError("", "太快了，慢一点儿");
			}
		}
	}
}
