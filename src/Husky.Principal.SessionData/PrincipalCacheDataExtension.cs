using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.Principal
{
	public static class PrincipalCacheDataExtension
	{
		private static CacheDataPool<CacheDataBag>? _pool;

		internal static string CacheKey(this IPrincipalUser principal) {
			return principal.IsAnonymous
				? principal.AnonymousId.ToString()
				: principal.Id.ToString();
		}

		internal static string CacheKeyDroppable(this IPrincipalUser principal) {
			return principal.IsAuthenticated
				? principal.AnonymousId.ToString()
				: principal.Id.ToString();
		}

		public static CacheDataBag CacheData(this IPrincipalUser principal) {
			var key = principal.CacheKey();
			_pool ??= new CacheDataPool<CacheDataBag>(principal.ServiceProvider.GetRequiredService<IMemoryCache>());
			_pool.Drop(principal.CacheKeyDroppable());
			return _pool.PickOrCreate(key, key => new CacheDataBag(principal));
		}

		public static void AbandonCache(this IPrincipalUser principal) {
			_pool ??= new CacheDataPool<CacheDataBag>(principal.ServiceProvider.GetRequiredService<IMemoryCache>());
			_pool.Drop(principal.Id.ToString());
			_pool.Drop(principal.AnonymousId.ToString());
		}
	}
}