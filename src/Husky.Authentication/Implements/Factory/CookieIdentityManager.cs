using System;
using Husky.Authentication.Abstractions;
using Microsoft.AspNetCore.Http;

namespace Husky.Authentication.Implements
{
	internal sealed class CookieIdentityManager : IIdentityManager
	{
		internal CookieIdentityManager(HttpContext httpContext, IdentityOptions options = null) {
			_httpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
			_options = (options ?? new IdentityOptions()).SolveUnassignedOptions(IdentityCarrier.Cookie);
		}

		HttpContext _httpContext;
		IdentityOptions _options;

		IIdentity IIdentityManager.ReadIdentity() {
			var cookie = _httpContext.Request.Cookies[_options.Key];
			if ( string.IsNullOrEmpty(cookie) ) {
				return null;
			}
			var identity = _options.Encryptor.Decrypt(cookie, _options.Token);
			if ( identity == null || identity.IsAnonymous || (_options.SessionMode && IsSessionLost()) ) {
				_httpContext.Response.Cookies.Delete(_options.Key);
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
			_httpContext.Response.Cookies.Append(
				key: _options.Key,
				value: _options.Encryptor.Encrypt(identity, _options.Token),
				options: new CookieOptions {
					HttpOnly = true,
					Expires = _options.Expires
				}
			);
			if ( _options.SessionMode ) {
				SetSession();
			}
		}

		string _sessionKey = "WEIXING_AUTH_SESSION_";

		void SetSession() => _httpContext.Response.Cookies.Append(_sessionKey, string.Empty, new CookieOptions { HttpOnly = true });
		bool IsSessionLost() => !_httpContext.Request.Cookies.ContainsKey(_sessionKey);

		void IIdentityManager.DeleteIdentity() {
			_httpContext.Response.Cookies.Delete(_options.Key);
			_httpContext.Response.Cookies.Delete(_sessionKey);
		}
	}
}
