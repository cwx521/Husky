using Husky.AspNetCore.Principal;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.AspNetCore.TwoFactor
{
	public static class PrincipalExtension
	{
		public static TwoFactorManager TwoFactor(this IPrincipalUser principal) {
			return principal.ServiceProvider.GetRequiredService<TwoFactorManager>();
		}
	}
}
