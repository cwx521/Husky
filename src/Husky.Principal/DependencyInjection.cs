using System;
using Husky.Principal;
using Husky.Principal.Implements;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Husky
{
	public static class DependencyInjection
	{
		public static HuskyDI AddPrincipal(this HuskyDI husky, IdentityCarrier carrier, IdentityOptions options = null) {
			var key = typeof(IPrincipalUser).FullName;

			husky.Services
				.AddScoped<IIdentityManager>(svc => {
					var http = svc.GetRequiredService<IHttpContextAccessor>().HttpContext;
					switch ( carrier ) {
						default: throw new ArgumentOutOfRangeException(nameof(carrier));
						case IdentityCarrier.Cookie: return new CookieIdentityManager(http, options);
						case IdentityCarrier.Header: return new HeaderIdentityManager(http, options);
						case IdentityCarrier.Session: return new SessionIdentityManager(http, options);
					}
				})
				.AddScoped<IPrincipalUser>(svc => {
					var http = svc.GetRequiredService<IHttpContextAccessor>().HttpContext;
					var identityManager = svc.GetRequiredService<IIdentityManager>();
					var principal = http.Items[key] as IPrincipalUser;
					if ( principal == null ) {
						principal = new PrincipalUser(identityManager, svc);
						http.Items.Add(key, principal);
					}
					return principal;
				});

			return husky;
		}
	}
}
