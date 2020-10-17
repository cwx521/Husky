using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.Principal
{
	public static class PrincipalSessionDataExtension
	{
		public static string SessionDataKey(this IPrincipalUser principal) {
			return principal.IsAnonymous
				? principal.AnonymousId.ToString()
				: principal.Id.ToString();
		}

		public static SessionDataContainer SessionData(this IPrincipalUser principal) {
			var key = principal.SessionDataKey();
			var cache = principal.ServiceProvider.GetRequiredService<IMemoryCache>();
			var pool = new SessionDataPool<SessionDataContainer>(cache);
			var dataBag = new SessionDataContainer(principal);
			return pool.PickOrCreate(key, dataBag);
		}

		public static void AbandonSessionData(this IPrincipalUser principal, SessionDataBag droppingBag) {
			var cache = principal.ServiceProvider.GetRequiredService<IMemoryCache>();
			var pool = new SessionDataPool<SessionDataContainer>(cache);

			if ( droppingBag != SessionDataBag.Anonymous ) {
				pool.Drop(principal.Id.ToString());
			}
			if ( droppingBag != SessionDataBag.LoggedOn ) {
				pool.Drop(principal.AnonymousId.ToString());
			}
		}
	}
}