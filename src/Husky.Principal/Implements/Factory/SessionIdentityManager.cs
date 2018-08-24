using System;
using Microsoft.AspNetCore.Http;

namespace Husky.Principal.Implements
{
	internal sealed class SessionIdentityManager : IIdentityManager
	{
		internal SessionIdentityManager(HttpContext httpContext, IdentityOptions options = null) {
			_httpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
			_options = (options ?? new IdentityOptions()).SolveUnassignedOptions(IdentityCarrier.Session);
		}

		private HttpContext _httpContext;
		private IdentityOptions _options;

		IIdentity IIdentityManager.ReadIdentity() {
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

		void IIdentityManager.SaveIdentity(IIdentity identity) {
			if ( identity == null ) {
				throw new ArgumentNullException(nameof(identity));
			}
			if ( identity.IsAnonymous ) {
				throw new ArgumentNullException($"{nameof(identity)}.{nameof(identity.IdString)} '{identity.IdString}' is not a athenticated value.");
			}
			_httpContext.Session.SetString(_options.Key, string.Concat(identity.IdString, '|', identity.DisplayName));
		}

		void IIdentityManager.DeleteIdentity() => _httpContext.Session.Remove(_options.Key);
	}
}
