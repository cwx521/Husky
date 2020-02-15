using System;
using System.Linq;
using Husky.Diagnostics.Data;
using Husky.Principal;
using Microsoft.AspNetCore.Http.Extensions;
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

				var db = context.HttpContext.RequestServices.GetRequiredService<DiagnosticsDbContext>();
				var principal = context.HttpContext.RequestServices.GetService<IPrincipalUser>();
				var userIdString = principal?.IdString;
				var userName = principal?.DisplayName ?? principal?.IdString ?? context.HttpContext.User?.Identity?.Name;
				var userIp = context.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();

				var exception = context.Exception;
				while ( exception.InnerException != null ) {
					exception = exception.InnerException;
				}

				var log = new ExceptionLog {
					HttpMethod = context.HttpContext.Request.Method,
					ExceptionType = exception.GetType().FullName,
					Message = exception.Message,
					Source = exception.Source,
					StackTrace = exception.StackTrace,
					Url = context.HttpContext.Request.GetDisplayUrl(),
					UserIdString = userIdString,
					UserName = userName,
					UserAgent = context.HttpContext.Request.UserAgent(),
					UserIp = userIp
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
			catch {
			}
		}
	}
}
