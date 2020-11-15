using System;
using System.Linq;
using System.Threading.Tasks;
using Husky.Diagnostics.Data;
using Husky.KeyValues;
using Husky.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Husky.Diagnostics
{
	public static class DiagnosticsDbContextExtensions
	{
		internal static async Task LogExceptionAsync(this IDiagnosticsDbContext db, Exception e, HttpContext? http, IPrincipalUser? principal) {
			principal ??= http?.RequestServices?.GetService<IPrincipalUser>();
			http ??= principal?.ServiceProvider?.GetService<IHttpContextAccessor>()?.HttpContext;

			var log = new ExceptionLog {
				ExceptionType = e.GetType().Name,
				Message = e.Message,
				Source = e.Source,
				StackTrace = e.StackTrace,
			};
			if ( principal != null ) {
				log.ReadValuesFromPrincipal(principal);
			}
			if ( http != null ) {
				log.ReadValuesFromHttpContext(http);
			}
			log.ComputeMd5Comparison();

			var repeating = db.ExceptionLogs
				.OrderByDescending(x => x.Id)
				.Where(x => x.Md5Comparison == log.Md5Comparison)
				.FirstOrDefault();

			if ( repeating == null ) {
				db.ExceptionLogs.Add(log);
			}
			else {
				repeating.Repeated++;
				repeating.LastTime = DateTime.Now;
			}
			await db.Normalize().SaveChangesAsync();
		}

		internal static async Task LogRequestAsync(this IDiagnosticsDbContext db, HttpContext? http, IPrincipalUser? principal) {
			if ( http == null ) {
				return;
			}

			principal ??= http.RequestServices.GetService<IPrincipalUser>();

			var log = new RequestLog();
			if ( principal != null ) {
				log.ReadValuesFromPrincipal(principal);
			}
			log.ReadValuesFromHttpContext(http);
			log.ComputeMd5Comparison();

			var keyValueManager = http.RequestServices.GetService<IKeyValueManager>();
			var seconds = keyValueManager?.LogRequestAsRepeatedIfVisitAgainWithinSeconds() ?? 60;

			var repeating = db.RequestLogs
				.OrderByDescending(x => x.Id)
				.Where(x => x.Md5Comparison == log.Md5Comparison)
				.Where(x => x.LastTime > DateTime.Now.AddSeconds(-seconds))
				.FirstOrDefault();

			if ( repeating == null ) {
				db.RequestLogs.Add(log);
			}
			else {
				repeating.Repeated++;
				repeating.LastTime = DateTime.Now;
			}
			await db.Normalize().SaveChangesAsync();
		}

		internal static async Task LogOperationAsync(this IDiagnosticsDbContext db, LogLevel logLevel, string message, IPrincipalUser? principal) {
			var log = new OperationLog {
				LogLevel = logLevel,
				Message = message
			};
			if ( principal != null ) {
				log.ReadValuesFromPrincipal(principal);
			}
			log.ComputeMd5Comparison();

			var keyValueManager = principal?.ServiceProvider?.GetService<IKeyValueManager>();
			var seconds = keyValueManager?.LogOperationAsRepeatedIfOperateAgainWithinSeconds() ?? 60;

			var repeating = db.OperationLogs
				.Where(x => x.Md5Comparison == log.Md5Comparison)
				.Where(x => x.LastTime > DateTime.Now.AddSeconds(-seconds))
				.FirstOrDefault();

			if ( repeating == null ) {
				db.OperationLogs.Add(log);
			}
			else {
				repeating.Repeated++;
				repeating.LastTime = DateTime.Now;
			}
			await db.Normalize().SaveChangesAsync();
		}
	}
}
