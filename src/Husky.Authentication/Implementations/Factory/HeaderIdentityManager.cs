using System;
using Husky.Authentication.Abstractions;
using Husky.Authentication.Implementations;
using Microsoft.AspNetCore.Http;

namespace Husky.Authentication.Implementations
{
	internal sealed class HeaderIdentityManager<T> : IIdentityManager<T> where T : IFormattable, IEquatable<T>
	{
		internal HeaderIdentityManager(HttpContext httpContext, IdentityOptions<T> options) {
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
		IdentityOptions<T> _options;

		Identity<T> IIdentityManager<T>.ReadIdentity() {
			var header = _httpContext.Request.Headers[_options.Key];
			if ( string.IsNullOrEmpty(header) ) {
				return null;
			}
			var identity = _options.Encryptor.Decrypt(header, _options.Token);
			if ( identity == null || !identity.IsAuthenticated ) {
				return null;
			}
			return identity;
		}

		void IIdentityManager<T>.SaveIdentity(Identity<T> identity) {
			if ( identity == null ) {
				throw new ArgumentNullException(nameof(identity));
			}
			if ( !identity.IsAuthenticated ) {
				throw new ArgumentException($"{nameof(identity)}.{nameof(identity.Id)} '{identity.Id}' is not an authenticated value.");
			}
			_httpContext.Response.Headers.Add(_options.Key, _options.Encryptor.Encrypt(identity, _options.Token));
		}

		void IIdentityManager<T>.DeleteIdentity() {
			_httpContext.Response.Headers.Remove(_options.Key);
		}

		public string GetEncryptedIdentityHeaderValue() => _httpContext.Request.Headers[_options.Key];
	}
}
