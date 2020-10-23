using System;
using Microsoft.AspNetCore.Http;

namespace Husky.Principal.Implementations
{
	internal sealed class HeaderIdentityManager : IIdentityManager
	{
		internal HeaderIdentityManager(IHttpContextAccessor httpContextAccessor, IdentityOptions? options = null) {
			_httpContext = httpContextAccessor.HttpContext;
			_options = options ?? new IdentityOptions();
		}

		private readonly HttpContext _httpContext;
		private readonly IdentityOptions _options;

		IIdentity IIdentityManager.ReadIdentity() {
			var primary = _httpContext.Request.Headers[_options.Key];
			var secondary = _httpContext.Request.Headers[_options.AnonymousIdKey];
			return IdentityReader.GetIdentity(primary, secondary, _options);
		}

		void IIdentityManager.SaveIdentity(IIdentity identity) {
			if ( identity == null ) {
				throw new ArgumentNullException(nameof(identity));
			}
			if ( _options.DedicateAnonymousIdStorage ) {
				_httpContext.Response.Headers[_options.AnonymousIdKey] = identity.AnonymousId.ToString();
			}
			_httpContext.Response.Headers[_options.Key] = _options.Encryptor.Encrypt(identity, _options.Token);
		}

		void IIdentityManager.DeleteIdentity() => _httpContext.Response.Headers.Remove(_options.Key);
	}
}
