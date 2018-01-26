using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.AspNetCore.Principal
{
	public static class PrincipaExtension
	{
		public static SessionDataContainer SessionData(this IPrincipalUser principal) {
			if ( principal.IsAnonymous ) {
				return null;
			}

			var cache = principal.ServiceProvider.GetRequiredService<IMemoryCache>();
			var pool = new SessionDataPool<SessionDataContainer>(cache);
			var sessionData = new SessionDataContainer(principal);

			return pool.PickOrCreate(principal.IdString, sessionData);
		}
	}
}