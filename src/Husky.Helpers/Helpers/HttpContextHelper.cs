using Microsoft.AspNetCore.Http;

namespace Husky
{
	public static class HttpContextHelper
	{
		public static bool IsAjaxRequest(this HttpRequest request) {
			return request.Headers["X-Requested-With"] == "XMLHttpRequest";
		}

		public static string UserAgent(this HttpRequest request) {
			return request.Headers["User-Agent"];
		}
	}
}
