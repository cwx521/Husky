using System;
using Husky.Authentication.Abstractions;
using Husky.Authentication.Implementations;
using Husky.Sugar;
using Microsoft.AspNetCore.Http;

namespace Husky.Authentication.Implementations
{
	internal sealed class SessionIdentityManager : IIdentityManager
	{
		internal SessionIdentityManager(HttpContext httpContext, IdentityOptions options) {
			if ( options == null ) {
				throw new ArgumentNullException(nameof(options));
			}
			if ( options.Token == null ) {
				throw new ArgumentException($"{nameof(options)}.{nameof(options.Token)} has no value assigned.");
			}

			_httpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
			_options = options.SolveUnassignedOptions(IdentityCarrier.Session);
		}

		HttpContext _httpContext;
		IdentityOptions _options;

		Identity IIdentityManager.ReadIdentity() {
			var combined = _httpContext.Session.GetString(_options.Key);
			if ( !string.IsNullOrEmpty(combined) ) {
				var i = combined.IndexOf('|');
				if ( i > 0 ) {
					return new Identity {
						IdString = combined.Substring(0, i),
						DisplayName = combined.Substring(i + 1)
					};
				}
			}
			return null;
		}

		void IIdentityManager.SaveIdentity(Identity identity) {
			if ( identity == null ) {
				throw new ArgumentNullException(nameof(identity));
			}
			if ( identity.IsAnonymous ) {
				throw new ArgumentNullException($"{nameof(identity)}.{nameof(identity.IdString)} '{identity.IdString}' is not a athenticated value.");
			}
			_httpContext.Session.SetString(_options.Key, string.Concat(identity.IdString, '|', identity.DisplayName));
		}

		void IIdentityManager.DeleteIdentity() {
			_httpContext.Session.Remove(_options.Key);
		}
	}
}
