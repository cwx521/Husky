using Microsoft.AspNetCore.Http;

namespace Husky
{
	public static class HttpContextHelper
	{
		public static bool IsAjaxRequest(this HttpRequest request) {
			return request.Headers["X-Requested-With"] == "XMLHttpRequest";
		}

		public static string Url(this HttpRequest request) {
			return request.Path + request.QueryString.Value;
		}

		public static string UrlBase(this HttpRequest request) {
			return (request.IsHttps ? "https://" : "http://") + request.Host;
		}

		public static string UserAgent(this HttpRequest request) {
			return request.Headers["User-Agent"];
		}
	}
}
