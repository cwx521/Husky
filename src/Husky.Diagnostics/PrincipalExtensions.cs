using System;
using System.Threading.Tasks;
using Husky.Diagnostics.Data;
using Husky.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Husky.Diagnostics
{
	public static class PrincipalExtensions
	{
		public static void LogException(this IPrincipalUser principal, Exception e) => LogExceptionAsync(principal, e).Wait();
		public static void LogRequest(this IPrincipalUser principal) => LogRequestAsync(principal).Wait();
		public static void LogOperation(this IPrincipalUser principal, string message, LogLevel logLevel = LogLevel.Warning) => LogOperationAsync(principal, message, logLevel).Wait();


		public static async Task LogExceptionAsync(this IPrincipalUser principal, Exception e) {
			var db = principal.ServiceProvider.GetRequiredService<IDiagnosticsDbContext>();
			var http = principal.ServiceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;
			await db.LogExceptionAsync(e, http, principal);
		}

		public static async Task LogRequestAsync(this IPrincipalUser principal) {
			var db = principal.ServiceProvider.GetRequiredService<IDiagnosticsDbContext>();
			var http = principal.ServiceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;
			await db.LogRequestAsync(http, principal);
		}

		public static async Task LogOperationAsync(this IPrincipalUser principal, string message, LogLevel logLevel = LogLevel.Warning) {
			var db = principal.ServiceProvider.GetRequiredService<IDiagnosticsDbContext>();
			await db.LogOperationAsync(principal, message, logLevel);
		}
	}
}
