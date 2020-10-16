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

		IIdentity? IIdentityManager.ReadIdentity() {
			if ( !_httpContext.Session.Keys.Contains(_options.Key) ) {
				return null;
			}
			var encrypted = _httpContext.Session.GetString(_options.Key);
			return _options.Encryptor.Decrypt(encrypted, _options.Token);
		}

		void IIdentityManager.SaveIdentity(IIdentity identity) {
			if ( identity == null ) {
				throw new ArgumentNullException(nameof(identity));
			}
			_httpContext.Session.SetString(
				_options.Key,
				_options.Encryptor.Encrypt(identity, _options.Token)
			);
		}

		void IIdentityManager.DeleteIdentity() => _httpContext.Session.Remove(_options.Key);
	}
}
