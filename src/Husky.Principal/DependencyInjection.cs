using System;
using Husky.Principal;
using Husky.Principal.Implementations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Husky
{
	public static class DependencyInjection
	{
		public static HuskyServiceHub AddPrincipal(this HuskyServiceHub husky, Action<IdentityOptions> optionBuilder) {
			var options = new IdentityOptions();
			optionBuilder(options);

			husky.Services.AddScoped<IIdentityManager>(svc => {
				var httpContext = svc.GetRequiredService<IHttpContextAccessor>().HttpContext!;
				return (options?.Carrier) switch {
					IdentityCarrier.HeaderAndCookie => new HybridIdentityManager(httpContext, options),
					IdentityCarrier.Header => new HeaderIdentityManager(httpContext, options),
					_ => new CookieIdentityManager(httpContext, options),
				};
			});
			husky.Services.AddScoped<IPrincipalUser, PrincipalUser>();

			return husky;
		}

		//public static HuskyServiceHub AddPrincipal(this HuskyServiceHub husky, Action<IdentityManagerSelector> setupAction) {
		//	var builder = new IdentityManagerSelector(husky.Services);
		//	setupAction(builder);
		//	husky.Services.AddScoped<IPrincipalUser, PrincipalUser>();
		//	return husky;
		//}

		//public class IdentityManagerSelector
		//{
		//	internal IdentityManagerSelector(IServiceCollection services) {
		//		_services = services;
		//	}

		//	private readonly IServiceCollection _services;

		//	public void Use<T>(IdentityOptions? options) where T : class, IIdentityManager {
		//		_services.AddSingleton(options ?? new IdentityOptions());
		//		_services.AddScoped<IIdentityManager, T>();
		//	}

		//	public void Use<T>(Action<IdentityOptions> optionBuilder) where T : class, IIdentityManager {
		//		var options = new IdentityOptions();
		//		optionBuilder(options);
		//		Use<T>(options);
		//	}

		//	public void Use<T>(Func<IServiceProvider, T> implementationFactory) where T : class, IIdentityManager {
		//		_services.AddScoped<IIdentityManager, T>(implementationFactory);
		//	}
		//}
	}
}
