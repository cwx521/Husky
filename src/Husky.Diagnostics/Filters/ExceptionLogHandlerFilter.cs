using System.Threading.Tasks;
using Husky.Diagnostics.Data;
using Husky.Principal;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.Diagnostics
{
	public sealed class ExceptionLogHandlerFilter : IAsyncExceptionFilter
	{
		public async Task OnExceptionAsync(ExceptionContext context) {
			try {
				var http = context.HttpContext;
				var db = http.RequestServices.GetRequiredService<IDiagnosticsDbContext>();
				var principal = http.RequestServices.GetService<IPrincipalUser>();

				var exception = context.Exception;
				while ( exception.InnerException != null ) {
					exception = exception.InnerException;
				}

				await db.LogExceptionAsync(exception, http, principal);
			}
			catch { }
		}
	}
}
