using System;
using Microsoft.AspNetCore.Http;

namespace Husky.Principal.Implementations
{
	internal sealed class SessionIdentityManager : IIdentityManager
	{
		internal SessionIdentityManager(HttpContext httpContext, IdentityOptions? options = null) {
			_httpContext = httpContext;
			_options = options ?? new IdentityOptions();
		}

		private readonly HttpContext _httpContext;
		private readonly IdentityOptions _options;

		IIdentity IIdentityManager.ReadIdentity() {
			var primary = _httpContext.Session.GetString(_options.Key);
			var secondary = _httpContext.Session.GetString(_options.AnonymousIdKey);
			return IdentityReader.GetIdentity(primary, secondary, _options);
		}

		void IIdentityManager.SaveIdentity(IIdentity identity) {
			if ( identity == null ) {
				throw new ArgumentNullException(nameof(identity));
			}
			if ( _options.DedicateAnonymousIdStorage ) {
				_httpContext.Session.SetString(_options.AnonymousIdKey, identity.AnonymousId.ToString());
			}
			_httpContext.Session.SetString(_options.Key, _options.Encryptor.Encrypt(identity, _options.Token));
		}

		void IIdentityManager.DeleteIdentity() => _httpContext.Session.Remove(_options.Key);
	}
}
