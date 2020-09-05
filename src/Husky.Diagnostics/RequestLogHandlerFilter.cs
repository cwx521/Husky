using System;
using Husky.Diagnostics.Data;
using Husky.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Husky.Diagnostics
{
	public class RequestLogHandlerFilter : IPageFilter
	{
		public void OnPageHandlerSelected(PageHandlerSelectedContext context) {
		}

		public void OnPageHandlerExecuted(PageHandlerExecutedContext context) {
		}

		public void OnPageHandlerExecuting(PageHandlerExecutingContext context) {
			var db = context.HttpContext.RequestServices.GetRequiredService<DiagnosticsDbContext>();
			var http = context.HttpContext.RequestServices.GetRequiredService<IHttpContextAccessor>().HttpContext;
			var principal = context.HttpContext.RequestServices.GetService<IPrincipalUser>();
			var url = http.Request.FullUrl();

			if ( http.Request.Query["norequestlog"].Count != 0 ) {
				return;
			}

			try {
				db.Add(new RequestLog {
					UserId = principal?.Id,
					UserName = principal?.DisplayName,
					HttpMethod = http.Request.Method,
					Data = JsonConvert.SerializeObject(http.Request.Form),
					UserAgent = http.Request.UserAgent(),
					IsAjax = http.Request.IsAjaxRequest(),
					Url = url, 
					Referrer = http.Request.Headers["Referer"].ToString(),
					UserIp = http.Connection.RemoteIpAddress.ToString()
				});
				db.SaveChanges();
			}
			catch { }
		}
	}
}
