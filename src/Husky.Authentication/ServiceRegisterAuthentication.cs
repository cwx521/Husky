using System;
using Husky.Authentication.Abstractions;
using Husky.Authentication.Implementations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.Authentication
{
	public static class ServiceRegisterAuthentication
	{
		public static IServiceCollection AddHuskyPrincipal<T>(this IServiceCollection services, IdentityCarrier carrier, IdentityOptions options)
			where T : class, IPrincipal {

			if ( options == null ) {
				throw new ArgumentNullException(nameof(options));
			}
			services.AddSingleton(serviceProvider => serviceProvider.CreateIdentityManager(carrier, options));
			services.AddScoped<T>();

			return services;
		}

		private static IIdentityManager CreateIdentityManager(this IServiceProvider serviceProvider, IdentityCarrier carrier, IdentityOptions options) {
			var httpContext = serviceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;
			switch ( carrier ) {
				default:
				case IdentityCarrier.Cookie: return new CookieIdentityManager(httpContext, options);
				case IdentityCarrier.Header: return new HeaderIdentityManager(httpContext, options);
				case IdentityCarrier.Session: return new SessionIdentityManager(httpContext, options);
			}
		}
	}
}
