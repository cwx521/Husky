using System;
using System.Linq;
using System.Threading.Tasks;
using Husky.Diagnostics.Data;
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
			if (principal != null) {
				log.ReadValuesFromPrincipal(principal);
			}
			if (http != null) {
				log.ReadValuesFromHttpContext(http);
			}
			log.ComputeMd5Comparison();

			var repeating = db.ExceptionLogs
				.Where(x => x.Md5Comparison == log.Md5Comparison)
				.OrderByDescending(x => x.Id)
				.FirstOrDefault();

			if (repeating == null) {
				db.ExceptionLogs.Add(log);
			}
			else {
				repeating.Repeated++;
				repeating.LastTime = DateTime.Now;
			}
			await db.Normalize().SaveChangesAsync();
		}

		internal static async Task LogRequestAsync(this IDiagnosticsDbContext db, HttpContext? http, IPrincipalUser? principal) {
			if (http == null) {
				return;
			}

			principal ??= http.RequestServices.GetService<IPrincipalUser>();

			var log = new RequestLog();
			if (principal != null) {
				log.ReadValuesFromPrincipal(principal);
			}
			log.ReadValuesFromHttpContext(http);

			db.RequestLogs.Add(log);
			await db.Normalize().SaveChangesAsync();
		}

		internal static async Task LogOperationAsync(this IDiagnosticsDbContext db, LogLevel logLevel, string message, IPrincipalUser? principal) {
			var log = new OperationLog {
				LogLevel = logLevel,
				Message = message
			};
			if (principal != null) {
				log.ReadValuesFromPrincipal(principal);
			}

			db.OperationLogs.Add(log);
			await db.Normalize().SaveChangesAsync();
		}

		internal static async Task LogPageViewAsync(this IDiagnosticsDbContext db, string pageId, IPrincipalUser? principal) {
			var log = new PageViewLog {
				PageId = pageId
			};
			if (principal != null) {
				log.ReadValuesFromPrincipal(principal);
			}

			db.PageViewLogs.Add(log);
			await db.Normalize().SaveChangesAsync();
		}
	}
}
