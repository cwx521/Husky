using Microsoft.AspNetCore.Http;

namespace Husky
{
	public static class HttpRequestHelper
	{
		public static string SchemeAndHost(this HttpRequest request) {
			return (request.IsHttps ? "https://" : "http://") + request.Host;
		}

		public static string Url(this HttpRequest request) {
			return request.PathBase + request.Path + request.QueryString.Value;
		}

		public static string FullUrl(this HttpRequest request) {
			return request.SchemeAndHost() + request.Url();
		}

		public static string UserAgent(this HttpRequest request) {
			return request.Headers["User-Agent"];
		}

		public static bool IsMobile(this HttpRequest request) {
			var userAgent = request.UserAgent();
			return userAgent.Contains("iPhone") || userAgent.Contains("Android");
		}

		public static bool IsWeChatBrowser(this HttpRequest request) {
			var userAgent = request.UserAgent();
			return userAgent.Contains("MicroMessenger");
		}

		public static bool IsXhr(this HttpRequest request) {
			return request.Headers["X-Requested-With"] == "XMLHttpRequest";
		}
		public static bool IsAjaxRequest(this HttpRequest request) {
			return request.Headers["X-Requested-With"] == "XMLHttpRequest";
		}
	}
}
