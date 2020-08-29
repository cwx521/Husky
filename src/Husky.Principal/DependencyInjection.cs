using System;
using Husky.Principal;
using Husky.Principal.Implements;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Husky
{
	public static class DependencyInjection
	{
		public static HuskyDI AddPrincipal(this HuskyDI husky, IdType idType, IdentityCarrier carrier, IdentityOptions options = null) {
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
					var p = http.Items[key] as IPrincipalUser;
					if ( p == null ) {
						p = svc.CreatePrincipalInstance(idType);
						http.Items.Add(key, p);
					}
					return p;
				});

			return husky;
		}

		private static IPrincipalUser CreatePrincipalInstance(this IServiceProvider svc, IdType idType) {
			var identityManager = svc.GetRequiredService<IIdentityManager>();
			switch ( idType ) {
				default: throw new ArgumentOutOfRangeException(nameof(idType));
				case IdType.Guid: return new Principal<Guid>(identityManager, svc);
				case IdType.Int: return new Principal<int>(identityManager, svc);
			}
		}
	}
}
