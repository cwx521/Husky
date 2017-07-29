using System;
using Husky.Authentication.Abstractions;
using Husky.Authentication.Implements;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.Authentication
{
	public static class ServiceRegisterAuthentication
	{
		public static IServiceCollection AddHuskyAuthentication(this IServiceCollection services, IdType idType, IdentityCarrier carrier, IdentityOptions options) => services
			.AddSingleton<IIdentityManager>(svc => {
				var httpContext = svc.GetRequiredService<IHttpContextAccessor>().HttpContext;
				switch ( carrier ) {
					default: throw new ArgumentOutOfRangeException(nameof(carrier));
					case IdentityCarrier.Cookie: return new CookieIdentityManager(httpContext, options);
					case IdentityCarrier.Header: return new HeaderIdentityManager(httpContext, options);
					case IdentityCarrier.Session: return new SessionIdentityManager(httpContext, options);
				}
			})
			.AddScoped<IPrincipal>(svc => {
				var identityManager = svc.GetRequiredService<IIdentityManager>(); ;
				switch ( idType ) {
					default: throw new ArgumentOutOfRangeException(nameof(idType));
					case IdType.Guid: return new Principal<Guid>(identityManager, svc);
					case IdType.Int: return new Principal<int>(identityManager, svc);
				}
			});
	}
}
