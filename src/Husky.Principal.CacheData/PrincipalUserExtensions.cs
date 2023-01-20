using System;
using Husky.KeyValues;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.Principal
{
	public static class PrincipalUserExtensions
	{
		private static CacheDataPool<CacheDictionaryBag>? _pool;
		private static int? _timeoutSeconds;

		internal static string CacheKey(this IPrincipalUser principal) {
			return principal.IsAnonymous
				? principal.AnonymousId.ToString()
				: principal.Id.ToString();
		}

		internal static string CacheKeyDroppable(this IPrincipalUser principal) {
			return principal.IsAuthenticated
				? principal.AnonymousId.ToString()  // authenticated ?	then the anonymous data bag can be dropped
				: principal.Id.ToString();          // anonymous?		then the id bag can be dropped
		}

		public static CacheDictionaryBag Cache(this IPrincipalUser principal) {
			var keyValues = principal.ServiceProvider.GetService<IKeyValueManager>();
			var memoryCache = principal.ServiceProvider.GetRequiredService<IMemoryCache>();

			_timeoutSeconds ??= keyValues?.PrincipalCacheDataBagWillExpireAfterSeconds() ?? 1800;
			_pool ??= new CacheDataPool<CacheDictionaryBag>(memoryCache) {
				Timeout = TimeSpan.FromSeconds(_timeoutSeconds.Value)
			};
			_pool.Drop(principal.CacheKeyDroppable());
			return _pool.PickOrCreate(principal.CacheKey(), key => new CacheDictionaryBag(key));
		}

		public static void AbandonCache(this IPrincipalUser principal) {
			if (_pool != null) {
				_pool.Drop(principal.Id.ToString());
				_pool.Drop(principal.AnonymousId.ToString());
				_pool.DropTimeout();
			}
		}
	}
}