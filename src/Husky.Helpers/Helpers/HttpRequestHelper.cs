using System;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

namespace Husky
{
	public static class HttpRequestHelper
	{
		public static string? RemoteIpv4(this HttpRequest httpRequest) {
			return httpRequest.HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString();
		}

		public static string ProtocolAndHost(this HttpRequest httpRequest) {
			return (httpRequest.IsHttps ? "https://" : "http://") + httpRequest.Host;
		}

		public static string Url(this HttpRequest httpRequest) {
			return httpRequest.PathBase + httpRequest.Path + Regex.Replace(httpRequest.QueryString.Value ?? "", @"[\?&]_=\d+$", "");
		}

		public static string FullUrl(this HttpRequest httpRequest) {
			return httpRequest.ProtocolAndHost() + httpRequest.Url();
		}

		public static string Referer(this HttpRequest httpRequest) {
			return httpRequest.Headers["Referer"].ToString();
		}

		public static string UserAgent(this HttpRequest httpRequest) {
			return httpRequest.Headers["User-Agent"].ToString();
		}

		public static bool IsMobile(this HttpRequest httpRequest) {
			var userAgent = httpRequest.UserAgent();
			return userAgent.Contains("iPhone", StringComparison.OrdinalIgnoreCase) || userAgent.Contains("Android", StringComparison.OrdinalIgnoreCase);
		}

		public static bool IsInWeChat(this HttpRequest httpRequest) {
			return httpRequest.UserAgent().Contains("MicroMessenger");
		}

		public static bool IsXhr(this HttpRequest httpRequest) {
			return httpRequest.Headers["X-Requested-With"] == "XMLHttpRequest";
		}
		public static bool IsAjaxRequest(this HttpRequest httpRequest) {
			return httpRequest.Headers["X-Requested-With"] == "XMLHttpRequest";
		}

		public static bool IsLocal(this HttpRequest httpRequest) {
			return httpRequest.Host.Host == "127.0.0.1" || httpRequest.Host.Host == "::1" || httpRequest.Host.Host == "localhost";
		}
	}
}
