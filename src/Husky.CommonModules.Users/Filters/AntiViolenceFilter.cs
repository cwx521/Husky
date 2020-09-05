using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.Principal.Filters
{
	public class AntiViolenceFilter : IPageFilter
	{
		public void OnPageHandlerSelected(PageHandlerSelectedContext context) {
		}

		public void OnPageHandlerExecuted(PageHandlerExecutedContext context) {
		}

		public void OnPageHandlerExecuting(PageHandlerExecutingContext context) {
			var principal = context.HttpContext.RequestServices.GetRequiredService<IPrincipalUser>();
			if ( principal.IsAnonymous ) {
				return;
			}
			if ( context.HttpContext.Request.Method == "GET" ) {
				return;
			}

			if ( principal.AntiViolenceTimer().AddMilliseconds(300) < DateTime.Now ) {
				principal.SetAntiViolenceTimer();
			}
			else {
				principal.SetAntiViolenceTimer(DateTime.Now.AddMilliseconds(300));
				context.Result = new ViewResult { ViewName = "_AntiViolence" };
			}
		}
	}
}
