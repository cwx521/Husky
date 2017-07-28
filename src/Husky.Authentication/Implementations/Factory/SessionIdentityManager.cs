using System;
using Husky.Authentication.Abstractions;
using Husky.Authentication.Implementations;
using Husky.Sugar;
using Microsoft.AspNetCore.Http;

namespace Husky.Authentication.Implementations
{
	internal sealed class SessionIdentityManager<T> : IIdentityManager<T> where T : IFormattable, IEquatable<T>
	{
		internal SessionIdentityManager(HttpContext httpContext, IdentityOptions<T> options) {
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
		IdentityOptions<T> _options;

		Identity<T> IIdentityManager<T>.ReadIdentity() {
			var combined = _httpContext.Session.GetString(_options.Key);
			if ( !string.IsNullOrEmpty(combined) ) {
				var i = combined.IndexOf('|');
				if ( i > 0 ) {
					return new Identity<T> {
						Id = combined.Substring(0, i).As<T>(),
						DisplayName = combined.Substring(i + 1)
					};
				}
			}
			return null;
		}

		void IIdentityManager<T>.SaveIdentity(Identity<T> identity) {
			if ( identity == null ) {
				throw new ArgumentNullException(nameof(identity));
			}
			if ( !identity.IsAuthenticated ) {
				throw new ArgumentNullException($"{nameof(identity)}.{nameof(identity.Id)} '{identity.Id}' is not a athenticated value.");
			}
			_httpContext.Session.SetString(_options.Key, string.Concat(identity.Id, '|', identity.DisplayName));
		}

		void IIdentityManager<T>.DeleteIdentity() {
			_httpContext.Session.Remove(_options.Key);
		}
	}
}
