using Husky.Principal;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.TwoFactor
{
	public static class PrincipalTwoFactorExtension
	{
		public static ITwoFactorManager TwoFactor(this IPrincipalUser principal) => principal.ServiceProvider.GetRequiredService<ITwoFactorManager>();
	}
}
