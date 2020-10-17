using System;
using System.Linq;
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

		private readonly string _anonymousKey = "HUSKY_AUTH_ANONYMOUS";

		IIdentity IIdentityManager.ReadIdentity() {
			var anonymous = _httpContext.Session.GetString(_anonymousKey);
			var logon = _httpContext.Session.GetString(_options.Key);
			return IdentityAnalysisHelper.GetIdentity(anonymous, logon, _options);
		}

		void IIdentityManager.SaveIdentity(IIdentity identity) {
			if ( identity == null ) {
				throw new ArgumentNullException(nameof(identity));
			}
			_httpContext.Session.SetString(_options.Key, _options.Encryptor.Encrypt(identity, _options.Token));
			_httpContext.Session.SetString(_anonymousKey, identity.AnonymousId.ToString());
		}

		void IIdentityManager.DeleteIdentity() => _httpContext.Session.Remove(_options.Key);
	}
}
