using System;
using Husky.AspNetCore.Principal;
using Husky.AspNetCore.Principal.Implements;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.AspNetCore
{
	public static class DI
	{
		public static HuskyDependencyInjectionHub AddPrincipal(this HuskyDependencyInjectionHub husky, IdType idType, IdentityCarrier carrier, IdentityOptions options = null) {
			var key = typeof(IPrincipalUser).FullName;

			husky.ServiceCollection
				.AddSingleton<IIdentityManager>(svc => {
					var httpContext = svc.GetRequiredService<IHttpContextAccessor>().HttpContext;
					switch ( carrier ) {
						default: throw new ArgumentOutOfRangeException(nameof(carrier));
						case IdentityCarrier.Cookie: return new CookieIdentityManager(httpContext, options);
						case IdentityCarrier.Header: return new HeaderIdentityManager(httpContext, options);
						case IdentityCarrier.Session: return new SessionIdentityManager(httpContext, options);
					}
				})
				.AddScoped(svc => {
					var http = svc.GetRequiredService<IHttpContextAccessor>().HttpContext;
					if ( !http.Items.ContainsKey(key) ) {
						http.Items.Add(key, svc.CreatePrincipalInstance(idType));
					}
					return http.Items[key] as IPrincipalUser;
				});

			return husky;
		}

		static IPrincipalUser CreatePrincipalInstance(this IServiceProvider svc, IdType idType) {
			var identityManager = svc.GetRequiredService<IIdentityManager>();
			switch ( idType ) {
				default: throw new ArgumentOutOfRangeException(nameof(idType));
				case IdType.Guid: return new Principal<Guid>(identityManager, svc);
				case IdType.Int: return new Principal<int>(identityManager, svc);
			}
		}
	}
}
