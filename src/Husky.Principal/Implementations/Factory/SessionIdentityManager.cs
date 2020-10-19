using System;
using Microsoft.AspNetCore.Http;

namespace Husky.Principal.Implements
{
	internal sealed class SessionIdentityManager : IIdentityManager
	{
		internal SessionIdentityManager(HttpContext httpContext, IdentityOptions? options = null) {
			_httpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
			_options = options ?? new IdentityOptions();
		}

		private readonly HttpContext _httpContext;
		private readonly IdentityOptions _options;

		IIdentity IIdentityManager.ReadIdentity() {
			var primary = _httpContext.Session.GetString(_options.Key);

			if ( !_options.DedicateAnonymousIdStorage ) {
				return _options.Encryptor.Decrypt(primary, _options.Token) ?? new Identity();
			}
			else {
				var secondary = _httpContext.Session.GetString(IdentityHelper.AnonymousKey);
				return IdentityHelper.GetIdentity(primary, secondary, _options);
			}
		}

		void IIdentityManager.SaveIdentity(IIdentity identity) {
			if ( identity == null ) {
				throw new ArgumentNullException(nameof(identity));
			}
			if ( _options.DedicateAnonymousIdStorage ) {
				_httpContext.Session.SetString(IdentityHelper.AnonymousKey, identity.AnonymousId.ToString());
			}
			_httpContext.Session.SetString(_options.Key, _options.Encryptor.Encrypt(identity, _options.Token));
		}

		void IIdentityManager.DeleteIdentity() => _httpContext.Session.Remove(_options.Key);
	}
}
