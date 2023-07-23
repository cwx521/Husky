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
			var token = _onHeader.ReadRawToken();
			if ( string.IsNullOrEmpty(token) ) {
				token = _onCookie.ReadRawToken();
			}
			return token;
		}

		IIdentity IIdentityManager.ReadIdentity() {
			var identity = _onHeader.ReadIdentity();
			if ( identity.IsAnonymous ) {
				identity = _onCookie.ReadIdentity();
			}
			return identity;
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
