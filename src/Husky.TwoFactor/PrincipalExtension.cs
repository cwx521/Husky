using Husky.Principal;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.TwoFactor
{
	public static class PrincipalExtension
	{
		public static TwoFactorManager TwoFactor(this IPrincipalUser principal) {
			return principal.ServiceProvider.GetRequiredService<TwoFactorManager>();
		}
	}
}
