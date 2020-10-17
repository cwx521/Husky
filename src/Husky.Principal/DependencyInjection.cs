using System;
using Husky.Principal;
using Husky.Principal.Implements;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Husky
{
	public static class DependencyInjection
	{
		public static HuskyDI AddIdentityManager(this HuskyDI husky, IdentityOptions? options = null) {
			husky.Services.AddScoped<IIdentityManager>(svc => {
				var httpContext = svc.GetRequiredService<IHttpContextAccessor>().HttpContext;
				switch ( options?.Carrier ) {
					default:
					case IdentityCarrier.Cookie: return new CookieIdentityManager(httpContext, options);
					case IdentityCarrier.Header: return new HeaderIdentityManager(httpContext, options);
					case IdentityCarrier.Session: return new SessionIdentityManager(httpContext, options);
				}
			});
			return husky;
		}

		public static HuskyDI AddIdentityManager(this HuskyDI husky, Action<IdentityOptions> setOptions) {
			var options = new IdentityOptions();
			setOptions(options);
			return husky.AddIdentityManager(options);
		}

		public static HuskyDI AddPrincipal(this HuskyDI husky, IdentityOptions? options = null) {
			husky.AddIdentityManager(options);
			husky.Services.AddScoped(serviceProvider => {

				var key = typeof(IPrincipalUser).FullName;
				var httpContext = serviceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;

				if ( !(httpContext.Items[key] is IPrincipalUser principal) ) {
					var identityManager = serviceProvider.GetRequiredService<IIdentityManager>();
					principal = new PrincipalUser(identityManager, serviceProvider);
					httpContext.Items.Add(key, principal);
				}
				return principal;
			});
			return husky;
		}

		public static HuskyDI AddPrincipal(this HuskyDI husky, Action<IdentityOptions> setOptions) {
			var options = new IdentityOptions();
			setOptions(options);
			return husky.AddPrincipal(options);
		}

		public static HuskyDI MapPrincipal<TPrincipalImplement>(this HuskyDI husky)
			where TPrincipalImplement : class, IPrincipalUser {
			husky.Services.AddScoped<IPrincipalUser, TPrincipalImplement>();
			return husky;
		}

		public static HuskyDI MapPrincipal<TPrincipalImplement>(this HuskyDI husky, Func<IServiceProvider, TPrincipalImplement> implementationFactory)
			where TPrincipalImplement : class, IPrincipalUser {
			husky.Services.AddScoped<IPrincipalUser, TPrincipalImplement>(implementationFactory);
			return husky;
		}
	}
}
