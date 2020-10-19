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
	public static class DiagnosticsDbContextExtension
	{
		public static async Task LogException(this IDiagnosticsDbContext db, Exception e, HttpContext? httpContext, IPrincipalUser? principal) {
			principal ??= httpContext?.RequestServices?.GetService<IPrincipalUser>();
			httpContext ??= principal?.ServiceProvider?.GetService<IHttpContextAccessor>()?.HttpContext;

			var log = new ExceptionLog {
				ExceptionType = e.GetType().FullName!,
				Message = e.Message,
				Source = e.Source,
				StackTrace = e.StackTrace,
			};
			if ( principal != null ) {
				log.EvaluateValuesFromPrincipal(principal);
			}
			if ( httpContext != null ) {
				log.EvaluateValuesFromHttpContext(httpContext);
			}
			log.ComputeMd5Comparison();

			var repeating = db.ExceptionLogs.FirstOrDefault(x => x.Md5Comparison == log.Md5Comparison);
			if ( repeating == null ) {
				db.ExceptionLogs.Add(log);
			}
			else {
				repeating.Repeated++;
				repeating.LastTime = DateTime.Now;
			}
			await db.Normalize().SaveChangesAsync();
		}

		public static async Task LogRequest(this IDiagnosticsDbContext db, HttpContext httpContext, IPrincipalUser principal) {
			var log = new RequestLog();
			log.EvaluateValuesFromPrincipal(principal);
			log.EvaluateValuesFromHttpContext(httpContext);
			log.ComputeMd5Comparison();

			var seconds = 60;
			var keyValueManager = httpContext.RequestServices.GetService<IKeyValueManager>();
			if ( keyValueManager != null ) {
				seconds = keyValueManager.GetOrAdd("LogAsRepeatedIfVisitAgainWithinSeconds", seconds);
			}

			var repeating = db.RequestLogs
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

		public static async Task LogOperation(this IDiagnosticsDbContext db, IPrincipalUser principal, string message, LogLevel logLevel) {
			var log = new OperationLog {
				LogLevel = logLevel,
				Message = message
			};
			log.EvaluateValuesFromPrincipal(principal);
			log.ComputeMd5Comparison();

			var seconds = 60;
			var keyValueManager = principal.ServiceProvider?.GetService<IKeyValueManager>();
			if ( keyValueManager != null ) {
				seconds = keyValueManager.GetOrAdd("LogAsRepeatedIfOperateAgainWithinSeconds", seconds);
			}

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
