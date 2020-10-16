using System;
using System.Linq;
using Husky.Diagnostics.Data;
using Husky.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Husky.Diagnostics
{
	public static class DiagnosticsLogManager
	{
		public static void LogException(this IDiagnosticsDbContext db, Exception e, IPrincipalUser? principal, HttpContext? httpContext = null) {
			httpContext ??= principal?.ServiceProvider?.GetService<IHttpContextAccessor>()?.HttpContext;

			var log = new ExceptionLog {
				HttpMethod = httpContext?.Request?.Method,
				ExceptionType = e.GetType().FullName!,
				Message = e.Message,
				Source = e.Source,
				StackTrace = e.StackTrace,
				Url = httpContext?.Request?.GetDisplayUrl(),
				UserId = principal?.Id,
				UserName = principal?.DisplayName ?? httpContext?.User?.Identity?.Name,
				UserAgent = httpContext?.Request?.UserAgent(),
				UserIp = httpContext?.Connection?.RemoteIpAddress?.MapToIPv4()?.ToString(),
			};
			log.ComputeMd5Comparison();

			var repeating = db.ExceptionLogs.FirstOrDefault(x => x.Md5Comparison == log.Md5Comparison);
			if ( repeating == null ) {
				db.ExceptionLogs.Add(log);
			}
			else {
				repeating.Repeated++;
				repeating.LastTime = DateTime.Now;
			}
			db.Normalize().SaveChanges();
		}

		public static void LogException(this IServiceProvider serviceProvider, Exception e) {
			using var scope = serviceProvider.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<IDiagnosticsDbContext>();

			HttpContext? httpContext = null;
			IPrincipalUser? principal = null;

			try {
				httpContext = scope.ServiceProvider.GetService<IHttpContextAccessor>()?.HttpContext;
				principal = scope.ServiceProvider.GetService<IPrincipalUser>();
			}
			catch { }

			db.LogException(e, principal, httpContext);
		}

		public static void LogRequest(this IDiagnosticsDbContext db, IPrincipalUser principal, HttpContext? httpContext = null) {
			httpContext ??= principal?.ServiceProvider?.GetService<IHttpContextAccessor>()?.HttpContext;
			if ( httpContext != null ) {

				var log = new RequestLog {
					UserId = principal?.Id,
					UserName = principal?.DisplayName,
					HttpMethod = httpContext.Request.Method,
					Data = httpContext.Request.Method == "GET" ? null : JsonConvert.SerializeObject(httpContext.Request.Form),
					UserAgent = httpContext.Request.UserAgent(),
					IsAjax = httpContext.Request.IsAjaxRequest(),
					Url = httpContext.Request.FullUrl(),
					Referrer = httpContext.Request.Headers["Referer"].ToString(),
					UserIp = httpContext.Connection.RemoteIpAddress.ToString()
				};
				log.ComputeMd5Comparison();

				var repeating = db.RequestLogs.FirstOrDefault(x => x.Md5Comparison == log.Md5Comparison && x.LastTime > DateTime.Now.AddSeconds(30));
				if ( repeating == null ) {
					db.RequestLogs.Add(log);
				}
				else {
					repeating.Repeated++;
					repeating.LastTime = DateTime.Now;
				}

				db.Normalize().SaveChanges();
			}
		}
	}
}
