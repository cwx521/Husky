using Husky.Diagnostics.Data;
using Husky.Principal;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.Diagnostics
{
	public sealed class ExceptionHandlerFilter : IExceptionFilter
	{
		public void OnException(ExceptionContext context) {
			try {
				if ( context.HttpContext == null ) {
					return;
				}

				var http = context.HttpContext;
				var db = http.RequestServices.GetRequiredService<DiagnosticsDbContext>();
				var principal = http.RequestServices.GetService<IPrincipalUser>();

				var exception = context.Exception;
				while ( exception.InnerException != null ) {
					exception = exception.InnerException;
				}

				db.Log(exception, http, principal);
			}
			catch { }
		}
	}
}
