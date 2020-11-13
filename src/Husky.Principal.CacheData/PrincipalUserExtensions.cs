using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.Principal
{
	public static class PrincipalUserExtensions
	{
		private static CacheDataPool<CacheDictionaryBag>? _pool;

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

		public static CacheDictionaryBag Cache(this IPrincipalUser principal) {
			var key = principal.CacheKey();
			_pool ??= new CacheDataPool<CacheDictionaryBag>(principal.ServiceProvider.GetRequiredService<IMemoryCache>());
			_pool.Drop(principal.CacheKeyDroppable());
			return _pool.PickOrCreate(key, key => new CacheDictionaryBag(principal));
		}

		public static void AbandonCache(this IPrincipalUser principal) {
			if ( _pool != null ) {
				_pool.Drop(principal.Id.ToString());
				_pool.Drop(principal.AnonymousId.ToString());
			}
		}
	}
}