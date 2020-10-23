using System.Threading.Tasks;
using Husky.Diagnostics.Data;
using Husky.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.Diagnostics
{
	public class RequestLogHandlerFilter : IAsyncPageFilter
	{

		public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context) {
			return Task.CompletedTask;
		}

		public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next) {
			var db = context.HttpContext.RequestServices.GetRequiredService<IDiagnosticsDbContext>();
			var http = context.HttpContext.RequestServices.GetRequiredService<IHttpContextAccessor>().HttpContext;
			var principal = context.HttpContext.RequestServices.GetService<IPrincipalUser>();

			if ( !http.Request.Query.ContainsKey("no_log") ) {
				try {
					await db.LogRequestAsync(http, principal);
				}
				catch { }
			}
			await next.Invoke();
		}
	}
}
