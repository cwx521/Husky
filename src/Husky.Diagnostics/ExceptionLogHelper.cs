using System;
using System.Linq;
using Husky.Diagnostics.Data;
using Husky.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.Diagnostics
{
	public static class ExceptionLogHelper
	{
		public static void Log(this DbContext db, Exception e, HttpContext http, IPrincipalUser principal) {
			var log = new ExceptionLog {
				HttpMethod = http?.Request?.Method,
				ExceptionType = e.GetType().FullName,
				Message = e.Message,
				Source = e.Source,
				StackTrace = e.StackTrace,
				Url = http?.Request?.GetDisplayUrl(),
				UserIdString = principal?.IdString,
				UserName = principal?.DisplayName ?? http?.User?.Identity?.Name,
				UserAgent = http?.Request?.UserAgent(),
				UserIp = http?.Connection?.RemoteIpAddress?.MapToIPv4()?.ToString()
			};
			log.ComputeMd5Comparison();

			var existedRow = db.Set<ExceptionLog>().FirstOrDefault(x => x.Md5Comparison == log.Md5Comparison);
			if ( existedRow == null ) {
				db.Add(log);
			}
			else {
				existedRow.Count++;
				existedRow.LastTime = DateTime.Now;
			}
			db.SaveChanges();
		}

		public static void Log(this IServiceProvider serviceProvider, Exception e) {
			using var scope = serviceProvider.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<DiagnosticsDbContext>();

			HttpContext http = null;
			IPrincipalUser principal = null;

			try {
				http = scope.ServiceProvider.GetService<IHttpContextAccessor>()?.HttpContext;
				principal = scope.ServiceProvider.GetService<IPrincipalUser>();
			}
			catch { }

			db.Log(e, http, principal);
		}
	}
}
