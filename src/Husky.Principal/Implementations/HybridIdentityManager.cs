using Microsoft.AspNetCore.Http;

namespace Husky.Principal.Implementations
{
	internal sealed class HybridIdentityManager : IIdentityManager
	{
		internal HybridIdentityManager(HttpContext httpContext, IIdentityOptions? options = null) {
			_options = options ?? new IdentityOptions();
			_onHeader = new HeaderIdentityManager(httpContext, _options);
			_onCookie = new CookieIdentityManager(httpContext, _options);
		}

		private readonly IIdentityOptions _options;
		private readonly IIdentityManager _onHeader;
		private readonly IIdentityManager _onCookie;

		IIdentityOptions IIdentityManager.Options => _options;

		string? IIdentityManager.ReadRawToken() {
			return _onHeader.ReadRawToken() ?? _onCookie.ReadRawToken();
		}

		IIdentity IIdentityManager.ReadIdentity() {
			return _onHeader.ReadIdentity() ?? _onCookie.ReadIdentity();
		}

		void IIdentityManager.SaveIdentity(IIdentity identity) {
			_onHeader.SaveIdentity(identity);
			_onCookie.SaveIdentity(identity);
		}

		void IIdentityManager.DeleteIdentity() {
			_onHeader.DeleteIdentity();
			_onCookie.DeleteIdentity();
		}
	}
}
