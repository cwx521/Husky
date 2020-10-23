using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

namespace Husky
{
	public static class HttpContextHelper
	{
		public static string RemoteIpv4(this HttpContext httpContext) {
			return httpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
		}

		public static string RemoteIpv4(this HttpRequest httpRequest) {
			return httpRequest.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
		}

		public static string SchemeAndHost(this HttpRequest httpRequest) {
			return (httpRequest.IsHttps ? "https://" : "http://") + httpRequest.Host;
		}

		public static string Url(this HttpRequest httpRequest) {
			return httpRequest.PathBase + httpRequest.Path + Regex.Replace(httpRequest.QueryString.Value, @"[\?&]_=\d+$", "");
		}

		public static string FullUrl(this HttpRequest httpRequest) {
			return httpRequest.SchemeAndHost() + httpRequest.Url();
		}

		public static string UserAgent(this HttpRequest httpRequest) {
			return httpRequest.Headers["User-Agent"];
		}

		public static bool IsMobile(this HttpRequest httpRequest) {
			var userAgent = httpRequest.UserAgent();
			return userAgent.Contains("iPhone") || userAgent.Contains("Android");
		}

		public static bool IsWeChatBrowser(this HttpRequest httpRequest) {
			return httpRequest.UserAgent().Contains("MicroMessenger");
		}

		public static bool IsXhr(this HttpRequest httpRequest) {
			return httpRequest.Headers["X-Requested-With"] == "XMLHttpRequest";
		}
		public static bool IsAjaxRequest(this HttpRequest httpRequest) {
			return httpRequest.Headers["X-Requested-With"] == "XMLHttpRequest";
		}

		public static bool IsLocalhost(this HttpRequest httpRequest) {
			return httpRequest.Host.Host == "127.0.0.1" || httpRequest.Host.Host == "::1" || httpRequest.Host.Host == "localhost";
		}
	}
}
