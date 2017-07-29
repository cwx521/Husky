using System;
using Husky.Authentication.Abstractions;
using Microsoft.AspNetCore.Http;

namespace Husky.Authentication.Implements
{
	internal sealed class HeaderIdentityManager : IIdentityManager
	{
		internal HeaderIdentityManager(HttpContext httpContext, IdentityOptions options) {
			if ( options == null ) {
				throw new ArgumentNullException(nameof(options));
			}
			if ( options.Token == null ) {
				throw new ArgumentException($"{nameof(options)}.{nameof(options.Token)} has no value assigned.");
			}

			_httpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
			_options = options.SolveUnassignedOptions(IdentityCarrier.Header);
		}

		HttpContext _httpContext;
		IdentityOptions _options;

		IIdentity IIdentityManager.ReadIdentity() {
			var header = _httpContext.Request.Headers[_options.Key];
			if ( string.IsNullOrEmpty(header) ) {
				return null;
			}
			var identity = _options.Encryptor.Decrypt(header, _options.Token);
			if ( identity == null || identity.IsAnonymous ) {
				return null;
			}
			return identity;
		}

		void IIdentityManager.SaveIdentity(IIdentity identity) {
			if ( identity == null ) {
				throw new ArgumentNullException(nameof(identity));
			}
			if ( identity.IsAnonymous ) {
				throw new ArgumentException($"{nameof(identity)}.{nameof(identity.IdString)} '{identity.IdString}' is not an authenticated value.");
			}
			_httpContext.Response.Headers.Add(_options.Key, _options.Encryptor.Encrypt(identity, _options.Token));
		}

		void IIdentityManager.DeleteIdentity() {
			_httpContext.Response.Headers.Remove(_options.Key);
		}

		public string GetEncryptedIdentityHeaderValue() => _httpContext.Request.Headers[_options.Key];
	}
}
