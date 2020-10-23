using System;
using System.Threading.Tasks;
using Husky.Diagnostics.Data;
using Husky.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Husky.Diagnostics
{
	public static class PrincipalUserExtensions
	{
		public static async Task LogException(this IPrincipalUser principal, Exception e) {
			var db = principal.ServiceProvider.GetRequiredService<IDiagnosticsDbContext>();
			var http = principal.ServiceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;
			await db.LogException(e, http, principal);
		}

		public static async Task LogRequest(this IPrincipalUser principal) {
			var db = principal.ServiceProvider.GetRequiredService<IDiagnosticsDbContext>();
			var http = principal.ServiceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;
			await db.LogRequest(http, principal);
		}

		public static async Task LogOperation(this IPrincipalUser principal, string message, LogLevel logLevel = LogLevel.Warning) {
			var db = principal.ServiceProvider.GetRequiredService<IDiagnosticsDbContext>();
			await db.LogOperation(principal, message, logLevel);
		}
	}
}
