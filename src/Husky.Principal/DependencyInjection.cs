using System;
using Husky.Principal;
using Husky.Principal.Implementations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Husky
{
	public static class DependencyInjection
	{
		public static HuskyInjector AddIdentityManager(this HuskyInjector husky, IdentityOptions? options = null) {
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

		public static HuskyInjector AddIdentityManager(this HuskyInjector husky, Action<IdentityOptions> setupAction) {
			var options = new IdentityOptions();
			setupAction(options);
			return husky.AddIdentityManager(options);
		}

		public static HuskyInjector AddPrincipal(this HuskyInjector husky, IdentityOptions? options = null) {
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

		public static HuskyInjector AddPrincipal(this HuskyInjector husky, Action<IdentityOptions> setupAction) {
			var options = new IdentityOptions();
			setupAction(options);
			return husky.AddPrincipal(options);
		}

		public static HuskyInjector MapPrincipal<TImplement>(this HuskyInjector husky)
			where TImplement : class, IPrincipalUser {
			husky.Services.AddScoped<IPrincipalUser, TImplement>();
			return husky;
		}

		public static HuskyInjector MapPrincipal<TImplement>(this HuskyInjector husky, Func<IServiceProvider, TImplement> implementationFactory)
			where TImplement : class, IPrincipalUser {
			husky.Services.AddScoped<IPrincipalUser, TImplement>(implementationFactory);
			return husky;
		}
	}
}
