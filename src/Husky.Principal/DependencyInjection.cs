using System;
using Husky.Principal;
using Husky.Principal.Implements;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Husky
{
	public static class DependencyInjection
	{
		public static HuskyDI AddPrincipal(this HuskyDI husky, IdentityCarrier carrier, IdentityOptions? options = null) {
			var key = typeof(IPrincipalUser).FullName;

			husky.Services
				.AddScoped<IIdentityManager>(svc => {
					var httpContext = svc.GetRequiredService<IHttpContextAccessor>().HttpContext;
					switch ( carrier ) {
						default: throw new ArgumentOutOfRangeException(nameof(carrier));
						case IdentityCarrier.Cookie: return new CookieIdentityManager(httpContext, options);
						case IdentityCarrier.Header: return new HeaderIdentityManager(httpContext, options);
						case IdentityCarrier.Session: return new SessionIdentityManager(httpContext, options);
					}
				})
				.AddScoped<IPrincipalUser>(svc => {
					var httpContext = svc.GetRequiredService<IHttpContextAccessor>().HttpContext;
					var identityManager = svc.GetRequiredService<IIdentityManager>();
					if ( !(httpContext.Items[key] is IPrincipalUser principal) ) {
						principal = new PrincipalUser(identityManager, svc);
						httpContext.Items.Add(key, principal);
					}
					return principal;
				});

			return husky;
		}
	}
}
