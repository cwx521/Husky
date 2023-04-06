using Microsoft.AspNetCore.Http;

namespace Husky.Principal.Implementations
{
	internal sealed class HybridIdentityManager : IIdentityManager
	{
		internal HybridIdentityManager(HttpContext httpContext, IIdentityOptions? options = null) {
			_onHeader = new HeaderIdentityManager(httpContext, options);
			_onCookie = new CookieIdentityManager(httpContext, options);
		}

		private readonly IIdentityManager _onHeader;
		private readonly IIdentityManager _onCookie;

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
