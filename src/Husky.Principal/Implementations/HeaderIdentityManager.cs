using Microsoft.AspNetCore.Http;

namespace Husky.Principal.Implementations
{
	internal sealed class HeaderIdentityManager : IIdentityManager
	{
		internal HeaderIdentityManager(HttpContext httpContext, IIdentityOptions? options = null) {
			_httpContext = httpContext;
			_options = options ?? new IdentityOptions();
		}

		private readonly HttpContext _httpContext;
		private readonly IIdentityOptions _options;

		IIdentityOptions IIdentityManager.Options => _options;

		string? IIdentityManager.ReadRawToken() {
			return _httpContext.Request.Headers[_options.IdKey];
		}

		IIdentity IIdentityManager.ReadIdentity() {
			var primary = _httpContext.Request.Headers[_options.IdKey];
			var secondary = _httpContext.Request.Headers[_options.AnonymousIdKey];
			return IdentityReader.GetIdentity(primary, secondary, _options);
		}

		void IIdentityManager.SaveIdentity(IIdentity identity) {
		}

		void IIdentityManager.DeleteIdentity() {
		}
	}
}
