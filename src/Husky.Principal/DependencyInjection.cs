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
				var http = svc.GetRequiredService<IHttpContextAccessor>().HttpContext;
				if ( http == null ) {
					throw new InvalidProgramException($"IHttpContextAccessor.HttpContext can not be null here.");
				}

				return (options?.Carrier) switch {
					IdentityCarrier.Header => new HeaderIdentityManager(http, options),
					IdentityCarrier.Session => new SessionIdentityManager(http, options),
					_ => new CookieIdentityManager(http, options),
				};
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
				var key = nameof(IPrincipalUser);
				var http = serviceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;

				if ( http == null ) {
					throw new InvalidProgramException("IHttpContextAccessor.HttpContext can not be null here.");
				}

				if ( http.Items[key] is not IPrincipalUser principal ) {
					var identityManager = serviceProvider.GetRequiredService<IIdentityManager>();
					principal = new PrincipalUser(identityManager, serviceProvider);
					http.Items.Add(key, principal);
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
	}
}
