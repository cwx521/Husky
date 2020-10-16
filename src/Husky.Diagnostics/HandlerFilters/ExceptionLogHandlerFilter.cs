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
				var httpContext = context.HttpContext;
				var db = httpContext.RequestServices.GetRequiredService<IDiagnosticsDbContext>();
				var principal = httpContext.RequestServices.GetService<IPrincipalUser>();

				var exception = context.Exception;
				while ( exception.InnerException != null ) {
					exception = exception.InnerException;
				}

				await db.LogException(exception, principal, httpContext);
			}
			catch { }
		}
	}
}
