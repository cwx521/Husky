using Husky.Diagnostics.Data;
using Husky.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.Diagnostics
{
	public class RequestLogHandlerFilter : IPageFilter
	{
		public void OnPageHandlerSelected(PageHandlerSelectedContext context) {
		}

		public void OnPageHandlerExecuted(PageHandlerExecutedContext context) {
		}

		public void OnPageHandlerExecuting(PageHandlerExecutingContext context) {
			var db = context.HttpContext.RequestServices.GetRequiredService<IDiagnosticsDbContext>();
			var httpContext = context.HttpContext.RequestServices.GetRequiredService<IHttpContextAccessor>().HttpContext;
			var principal = context.HttpContext.RequestServices.GetService<IPrincipalUser>();

			if ( !httpContext.Request.Query.ContainsKey("no_log") ) {
				try {
					db.LogRequest(principal, httpContext);
				}
				catch { }
			}
		}
	}
}
