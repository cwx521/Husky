using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.Principal
{
	public static class PrincipalCacheDataExtension
	{
		internal static string CacheKey(this IPrincipalUser principal) {
			return principal.IsAnonymous
				? principal.AnonymousId.ToString()
				: principal.Id.ToString();
		}

		public static CacheDataContainer CacheData(this IPrincipalUser principal) {
			var key = principal.CacheKey();
			var cache = principal.ServiceProvider.GetRequiredService<IMemoryCache>();
			var pool = new CacheDataPool<CacheDataContainer>(cache);
			var dataBag = new CacheDataContainer(principal);
			if ( principal.IsAuthenticated ) {
				pool.Drop(principal.AnonymousId.ToString());
			}
			return pool.PickOrCreate(key, dataBag);
		}

		public static void AbandonCache(this IPrincipalUser principal) {
			var cache = principal.ServiceProvider.GetRequiredService<IMemoryCache>();
			var pool = new CacheDataPool<CacheDataContainer>(cache);
			pool.Drop(principal.Id.ToString());
			pool.Drop(principal.AnonymousId.ToString());
		}
	}
}