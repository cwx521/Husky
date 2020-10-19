using System;
using Microsoft.AspNetCore.Http;

namespace Husky.Principal.Implements
{
	internal sealed class CookieIdentityManager : IIdentityManager
	{
		internal CookieIdentityManager(HttpContext httpContext, IdentityOptions? options = null) {
			_httpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
			_options = options ?? new IdentityOptions();
		}

		private readonly HttpContext _httpContext;
		private readonly IdentityOptions _options;


		IIdentity IIdentityManager.ReadIdentity() {
			_httpContext.Request.Cookies.TryGetValue(_options.Key, out var primary);

			if ( !_options.DedicateAnonymousIdStorage ) {
				return _options.Encryptor.Decrypt(primary, _options.Token) ?? new Identity();
			}
			else {
				_httpContext.Request.Cookies.TryGetValue(IdentityHelper.AnonymousKey, out var secondary);
				var identity = IdentityHelper.GetIdentity(primary, secondary, _options);
				if ( _options.SessionMode && IsSessionLost() ) {
					identity.Id = 0;
				}
				return identity;
			}
		}

		void IIdentityManager.SaveIdentity(IIdentity identity) {
			if ( identity == null ) {
				throw new ArgumentNullException(nameof(identity));
			}
			if ( !_httpContext.Response.HasStarted ) {
				_httpContext.Response.Cookies.Append(
					key: _options.Key,
					value: _options.Encryptor.Encrypt(identity, _options.Token),
					options: new CookieOptions {
						Expires = _options.Expires
					}
				);
				if ( _options.DedicateAnonymousIdStorage ) {
					_httpContext.Response.Cookies.Append(
						key: IdentityHelper.AnonymousKey,
						value: identity.AnonymousId.ToString()
					);
				}
				if ( _options.SessionMode ) {
					PlantSession();
				}
			}
		}

		private readonly string _browserLifeKey = "HUSKY_AUTH_BROWSER_LIFE";
		private void PlantSession() => _httpContext.Response.Cookies.Append(_browserLifeKey, DateTime.Now.Ticks.ToString());
		private bool IsSessionLost() => !_httpContext.Request.Cookies.ContainsKey(_browserLifeKey);

		void IIdentityManager.DeleteIdentity() {
			_httpContext.Response.Cookies.Delete(_options.Key);
			_httpContext.Response.Cookies.Delete(_browserLifeKey);
		}
	}
}
