using System;
using System.Linq;
using Husky.Diagnostics.Data;
using Husky.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.Diagnostics
{
	public static class ExceptionLogHelper
	{
		public static void Log(this IServiceProvider serviceProvider, Exception e) {
			using ( var scope = serviceProvider.CreateScope() ) {
				HttpContext http = null;
				IPrincipalUser principal = null;

				try {
					http = scope.ServiceProvider.GetService<IHttpContextAccessor>()?.HttpContext;
					principal = scope.ServiceProvider.GetService<IPrincipalUser>();
				}
				catch { }

				var db = scope.ServiceProvider.GetRequiredService<DiagnosticsDbContext>();
				var log = new ExceptionLog {
					HttpMethod = http?.Request?.Method,
					ExceptionType = e.GetType().FullName,
					Message = e.Message,
					Source = e.Source,
					StackTrace = e.StackTrace,
					Url = http?.Request?.GetDisplayUrl(),
					UserIdString = principal?.IdString,
					UserName = principal?.DisplayName,
					UserAgent = http?.Request?.UserAgent(),
					UserIp = http?.Connection?.RemoteIpAddress?.MapToIPv4()?.ToString()
				};
				log.ComputeMd5Comparison();

				var existedRow = db.ExceptionLogs.FirstOrDefault(x => x.Md5Comparison == log.Md5Comparison);
				if ( existedRow == null ) {
					db.Add(log);
				}
				else {
					existedRow.Count++;
					existedRow.LastTime = DateTime.Now;
				}
				db.SaveChanges();
			}
		}
	}
}
