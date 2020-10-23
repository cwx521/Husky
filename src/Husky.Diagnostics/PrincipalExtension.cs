using Husky.Principal;

namespace Husky.Diagnostics
{
	public static class PrincipalExtension
	{
		public static DiagnosticsLogger ShoppingCart(this IPrincipalUser principal) {
			return new DiagnosticsLogger(principal);
		}
	}
}
