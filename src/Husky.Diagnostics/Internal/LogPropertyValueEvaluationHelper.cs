using System.Linq;
using System.Text.Json;
using Husky.Diagnostics.Data;
using Husky.Principal;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.Diagnostics
{
	internal static class LogPropertyValueEvaluationHelper
	{
		internal static void ReadValuesFromHttpContext(this HttpLevelLogBase log, HttpContext http) {
			var antiforgery = http.RequestServices.GetService<IAntiforgery>()?.GetTokens(http).FormFieldName ?? "__RequestVerificationToken";

			log.HttpMethod = http.Request.Method;
			log.Url = http.Request.FullUrl();
			log.Referrer = http.Request.Headers["Referer"].ToString();
			log.Data = http.Request.HasFormContentType ? JsonSerializer.Serialize(http.Request.Form.Where(x => x.Key != antiforgery)) : null;
			log.UserAgent = http.Request.UserAgent();
			log.UserIp = http.Connection?.RemoteIpAddress?.ToString();
			log.IsAjax = http.Request.IsAjaxRequest();
		}

		internal static void ReadValuesFromPrincipal(this LogBase log, IPrincipalUser principal) {
			log.AnonymousId = principal.AnonymousId;
			log.UserId = principal.Id;
			log.UserName = principal.DisplayName;
		}
	}
}
