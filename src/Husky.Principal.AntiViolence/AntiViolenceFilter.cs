using System;
using Microsoft.AspNetCore.Mvc;
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

		private void OnExecuting(FilterContext context) {
			var principal = context.HttpContext.RequestServices.GetRequiredService<IPrincipalUser>();
			if ( principal.IsAnonymous ) {
				return;
			}
			if ( context.HttpContext.Request.Method == "GET" ) {
				return;
			}

			// POST too fast will be judged as violence
			// Within milliseconds 
			// todo: put this to configuration
			const int ms = 300;

			var antiViolence = new AntiViolenceBlocker(principal);

			if ( antiViolence.GetTimer().AddMilliseconds(ms) < DateTime.Now ) {
				antiViolence.SetTimer();
			}
			else {
				antiViolence.SetTimer(DateTime.Now.AddMilliseconds(ms));
				context.ModelState.AddModelError("", "操作太快了，慢一点儿");
			}
		}
	}
}
