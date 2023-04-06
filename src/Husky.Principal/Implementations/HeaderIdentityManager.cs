using System;
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

		string? IIdentityManager.ReadRawToken() {
			return _httpContext.Request.Headers[_options.IdKey];
		}

		IIdentity IIdentityManager.ReadIdentity() {
			var primary = _httpContext.Request.Headers[_options.IdKey];
			var secondary = _httpContext.Request.Headers[_options.AnonymousIdKey];
			return IdentityReader.GetIdentity(primary, secondary, _options);
		}

		void IIdentityManager.SaveIdentity(IIdentity identity) {
			if (identity == null) {
				throw new ArgumentNullException(nameof(identity));
			}
			if (identity.IsAuthenticated) {
				_httpContext.Response.Headers[_options.IdKey] = _options.Encryptor.Encrypt(identity, _options.Salt);
			}
			if (_options.DedicateAnonymousIdStorage) {
				_httpContext.Response.Headers[_options.AnonymousIdKey] = identity.AnonymousId.ToString();
			}
		}

		void IIdentityManager.DeleteIdentity() => _httpContext.Response.Headers.Remove(_options.IdKey);
	}
}
