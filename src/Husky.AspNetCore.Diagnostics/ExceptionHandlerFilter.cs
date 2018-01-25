using System;
using System.Linq;
using Husky.AspNetCore.Principal;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.AspNetCore.Diagnostics
{
	public sealed class ExceptionHandlerFilter : IExceptionFilter
	{
		public void OnException(ExceptionContext context) {
			try {
				var db = context.HttpContext.RequestServices.GetRequiredService<DiagnosticsDbContext>();
				var principal = context.HttpContext.RequestServices.GetService<IPrincipalUser>();
				var userName = principal?.DisplayName ?? context.HttpContext.User?.Identity?.Name;

				var log = new ExceptionLog {
					HttpMethod = context.HttpContext.Request.Method,
					ExceptionType = context.Exception.GetType().FullName,
					Message = context.Exception.Message,
					Source = context.Exception.Source,
					StackTrace = context.Exception.StackTrace,
					Url = context.HttpContext.Request.GetDisplayUrl(),
					UserName = userName,
					UserAgent = context.HttpContext.Request.UserAgent()
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
