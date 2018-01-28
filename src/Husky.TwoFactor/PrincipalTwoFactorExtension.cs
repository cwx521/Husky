using Husky.Principal;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.TwoFactor
{
	public static class PrincipalTwoFactorExtension
	{
		public static TwoFactorManager TwoFactor(this IPrincipalUser principal) {
			return principal.ServiceProvider.GetRequiredService<TwoFactorManager>();
		}
	}
}
