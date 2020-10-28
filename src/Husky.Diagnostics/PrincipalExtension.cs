using Husky.Principal;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.Diagnostics
{
	public static class PrincipalExtension
	{
		public static IDiagnosticsLogger Logger(this IPrincipalUser principal) => principal.ServiceProvider.GetRequiredService<IDiagnosticsLogger>();
	}
}
