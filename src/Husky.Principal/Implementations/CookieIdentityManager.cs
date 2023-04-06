using System;
using Microsoft.AspNetCore.Http;

namespace Husky.Principal.Implementations
{
	internal sealed class CookieIdentityManager : IIdentityManager
	{
		internal CookieIdentityManager(HttpContext httpContext, IIdentityOptions? options = null) {
			_httpContext = httpContext;
			_options = options ?? new IdentityOptions();
		}

		private readonly HttpContext _httpContext;
		private readonly IIdentityOptions _options;

		string? IIdentityManager.ReadRawToken() {
			_httpContext.Request.Cookies.TryGetValue(_options.IdKey, out var raw);
			return raw;
		}

		IIdentity IIdentityManager.ReadIdentity() {
			_httpContext.Request.Cookies.TryGetValue(_options.IdKey, out var primary);
			_httpContext.Request.Cookies.TryGetValue(_options.AnonymousIdKey, out var secondary);

			var identity = IdentityReader.GetIdentity(primary, secondary, _options);
			if (_options.SessionMode && IsLifeSessionLost()) {
				identity.Id = 0;
			}
			return identity;
		}

		void IIdentityManager.SaveIdentity(IIdentity identity) {
			if (identity == null) {
				throw new ArgumentNullException(nameof(identity));
			}
			if (!_httpContext.Response.HasStarted) {
				if (identity.IsAuthenticated) {
					_httpContext.Response.Cookies.Append(
						key: _options.IdKey,
						value: _options.Encryptor.Encrypt(identity, _options.Salt),
						options: new CookieOptions {
							Expires = _options.Expires
						}
					);
				}
				if (_options.DedicateAnonymousIdStorage) {
					_httpContext.Response.Cookies.Append(
						key: _options.AnonymousIdKey,
						value: identity.AnonymousId.ToString(),
						options: new CookieOptions {
							Expires = DateTime.Now.AddYears(10)
						}
					);
				}
				if (_options.SessionMode) {
					PlantLifeSession();
				}
			}
		}

		private const string _browserLifeKey = "HUSKY_AUTH_BROWSER_LIFE";
		private void PlantLifeSession() => _httpContext.Response.Cookies.Append(_browserLifeKey, DateTime.Now.Ticks.ToString());
		private bool IsLifeSessionLost() => !_httpContext.Request.Cookies.ContainsKey(_browserLifeKey);

		void IIdentityManager.DeleteIdentity() {
			_httpContext.Response.Cookies.Delete(_options.IdKey);
			_httpContext.Response.Cookies.Delete(_browserLifeKey);
		}
	}
}
