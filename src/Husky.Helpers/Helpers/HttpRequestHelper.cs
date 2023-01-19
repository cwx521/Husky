using System;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

namespace Husky
{
	public static class HttpRequestHelper
	{
		public static string? RemoteIpv4(this HttpRequest request) {
			return request.HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString();
		}

		public static string ProtocolAndHost(this HttpRequest request) {
			return (request.IsHttps ? "https://" : "http://") + request.Host;
		}

		public static string Url(this HttpRequest request) {
			return request.PathBase + request.Path + Regex.Replace(request.QueryString.Value ?? "", @"[\?&]_=\d+$", "");
		}

		public static string FullUrl(this HttpRequest request) {
			return request.ProtocolAndHost() + request.Url();
		}

		public static string Referer(this HttpRequest request) {
			return request.Headers["Referer"].ToString();
		}

		public static string UserAgent(this HttpRequest request) {
			return request.Headers["User-Agent"].ToString();
		}

		public static bool IsMobile(this HttpRequest request) {
			var userAgent = request.UserAgent();
			return userAgent.Contains("iPhone", StringComparison.OrdinalIgnoreCase) ||
				   userAgent.Contains("Android", StringComparison.OrdinalIgnoreCase) ||
				   userAgent.Contains("HarmonyOS", StringComparison.OrdinalIgnoreCase);
		}

		public static bool IsInWeChat(this HttpRequest request) {
			return request.UserAgent().Contains("MicroMessenger", StringComparison.OrdinalIgnoreCase);
		}

		public static bool IsXhr(this HttpRequest request) {
			return request.Headers["X-Requested-With"] == "XMLHttpRequest";
		}
		public static bool IsAjaxRequest(this HttpRequest request) {
			return request.Headers["X-Requested-With"] == "XMLHttpRequest";
		}

		public static bool IsLocal(this HttpRequest request) {
			return request.Host.Host == "127.0.0.1" || request.Host.Host == "::1" || request.Host.Host == "localhost";
		}
	}
}
