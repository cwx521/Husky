using System.Net.Http;

namespace Husky
{
	public static class DefaultHttpClient
	{
		private static HttpClient? _httpClient;
		public static HttpClient Instance {
			get {
				if ( _httpClient == null ) {
					_httpClient = new HttpClient();
				}
				return _httpClient;
			}
		}
	}
}
