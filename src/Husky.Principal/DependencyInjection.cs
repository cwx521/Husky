using System;
using Husky.Principal;
using Husky.Principal.Implements;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Husky
{
	public static class DependencyInjection
	{
		public static HuskyDI AddIdentityManager(this HuskyDI husky, IdentityCarrier carrier, IdentityOptions? options = null) {
			husky.Services.AddScoped<IIdentityManager>(svc => {
				var httpContext = svc.GetRequiredService<IHttpContextAccessor>().HttpContext;
				switch ( carrier ) {
					default: throw new ArgumentOutOfRangeException(nameof(carrier));
					case IdentityCarrier.Cookie: return new CookieIdentityManager(httpContext, options);
					case IdentityCarrier.Header: return new HeaderIdentityManager(httpContext, options);
					case IdentityCarrier.Session: return new SessionIdentityManager(httpContext, options);
				}
			});
			return husky;
		}

		public static HuskyDI AddPrincipal(this HuskyDI husky, IdentityCarrier carrier, IdentityOptions? options = null) {
			husky.AddIdentityManager(carrier, options);
			husky.Services.AddScoped<IPrincipalUser>(serviceProvider => {
				var key = typeof(IPrincipalUser).FullName;
				var httpContext = serviceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;
				var identityManager = serviceProvider.GetRequiredService<IIdentityManager>();
				if ( !(httpContext.Items[key] is IPrincipalUser principal) ) {
					principal = new PrincipalUser(identityManager, serviceProvider);
					httpContext.Items.Add(key, principal);
				}
				return principal;
			});
			return husky;
		}

		public static HuskyDI AddPrincipal(this HuskyDI husky, Func<IServiceProvider, IPrincipalUser> implement) {
			husky.Services.AddScoped<IPrincipalUser>(implement);
			return husky;
		}

		public static HuskyDI AddPrincipal<TPrincipalImplement>(this HuskyDI husky)
			where TPrincipalImplement : class, IPrincipalUser {
			husky.Services.AddScoped<IPrincipalUser, TPrincipalImplement>();
			return husky;
		}
	}
}
