using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.Principal.SessionData
{
	public static class PrincipalSessionDataExtension
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

		public static void AbandonSessionData(this IPrincipalUser principal) {
			var cache = principal.ServiceProvider.GetRequiredService<IMemoryCache>();
			var pool = new SessionDataPool<SessionDataContainer>(cache);
			pool.Drop(principal.IdString);
		}
	}
}