using System;
using Husky.Principal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.Users.Filters
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

			// POST too fast will be judged as violence
			// Within milliseconds 
			const int ms = 300;

			if ( principal.AntiViolenceTimer().AddMilliseconds(ms) < DateTime.Now ) {
				principal.SetAntiViolenceTimer();
			}
			else {
				principal.SetAntiViolenceTimer(DateTime.Now.AddMilliseconds(ms));
				context.Result = new ViewResult { ViewName = "_AntiViolence" };
			}
		}
	}
}
