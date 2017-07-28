using System;
using Husky.Authentication.Abstractions;
using Husky.Authentication.Implementations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.Authentication
{
	public static class ServiceProviderExtensions
	{
		public static IServiceCollection AddHuskyPrincipal<TPrincipal, TKey>(IServiceCollection services, IdentityCarrier carrier, IdentityOptions<TKey> options)
			where TPrincipal : class, IPrincipal<TKey>
			where TKey : IFormattable, IEquatable<TKey> {

			if ( options == null ) {
				throw new ArgumentNullException(nameof(options));
			}
			services.AddSingleton(serviceProvider => serviceProvider.CreateIdentityManager(carrier, options));
			services.AddScoped<TPrincipal>();

			return services;
		}

		private static IIdentityManager<TKey> CreateIdentityManager<TKey>(this IServiceProvider serviceProvider, IdentityCarrier carrier, IdentityOptions<TKey> options)
			where TKey : IFormattable, IEquatable<TKey> {

			var httpContext = serviceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;
			switch ( carrier ) {
				default:
				case IdentityCarrier.Cookie: return new CookieIdentityManager<TKey>(httpContext, options);
				case IdentityCarrier.Header: return new HeaderIdentityManager<TKey>(httpContext, options);
				case IdentityCarrier.Session: return new SessionIdentityManager<TKey>(httpContext, options);
			}
		}
	}
}
