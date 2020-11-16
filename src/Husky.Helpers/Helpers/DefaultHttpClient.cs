using System.Net.Http;

namespace Husky
{
	public static class DefaultHttpClient
	{
		private static HttpClient? _httpClient;
		public static HttpClient Instance => _httpClient ??= new HttpClient();
	}
}