using System;
using System.Collections.Concurrent;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;

namespace Husky.Principal
{
	internal class CacheDataPool<T> where T : class, ICacheDataBag
	{
		internal CacheDataPool(IMemoryCache memoryCache) {
			_memoryCache = memoryCache;
		}

		private readonly IMemoryCache _memoryCache;
		private readonly string _cacheKeyOfPool = "Pool_" + typeof(T).Name;

		internal TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(30);

		internal T? Pick(string key) {
			if ( key == null ) {
				throw new ArgumentNullException(nameof(key));
			}
			var pool = GetPool();
			if ( pool != null ) {
				if ( pool.TryGetValue(key, out var bag) ) {
					if ( bag.ActiveTime.Add(Timeout) > DateTime.Now ) {
						bag.ActiveTime = DateTime.Now;
						return bag;
					}
					Drop(key);
				}
			}
			return default;
		}

		internal void Put(T bag) {
			DropTimeout(Timeout);
			bag.ActiveTime = DateTime.Now;
			EnsureGetPool().AddOrUpdate(bag.Key, bag, (_, _) => bag);
		}

		internal T PickOrCreate(string key, Func<string, T> createBag) {
			if ( key == null ) {
				throw new ArgumentNullException(nameof(key));
			}
			if ( createBag == null ) {
				throw new ArgumentNullException(nameof(createBag));
			}

			var pool = EnsureGetPool();

			if ( pool.TryGetValue(key, out var bag) && bag.ActiveTime.Add(Timeout) > DateTime.Now ) {
				bag.ActiveTime = DateTime.Now;
				return bag;
			}

			var created = createBag(key);
			created.ActiveTime = DateTime.Now;

			pool.AddOrUpdate(created.Key, created, (_, _) => created);
			return created;
		}

		internal void Drop(string key) {
			if ( key == null ) {
				throw new ArgumentNullException(nameof(key));
			}
			GetPool()?.TryRemove(key, out _);
		}

		internal void DropAll() => _memoryCache.Remove(_cacheKeyOfPool);

		internal void DropTimeout(TimeSpan timeout) {
			var pool = GetPool();
			if ( pool != null ) {
				var keys = new string[pool.Count];
				pool.Keys.CopyTo(keys, 0);

				keys.AsParallel().ForAll(i => {
					if ( pool.TryGetValue(i, out var bag) && bag.ActiveTime.Add(timeout) < DateTime.Now ) {
						pool.TryRemove(i, out _);
					}
				});
			}
		}

		private ConcurrentDictionary<string, T> GetPool() => _memoryCache.Get<ConcurrentDictionary<string, T>>(_cacheKeyOfPool);

		private ConcurrentDictionary<string, T> EnsureGetPool() {
			return _memoryCache.GetOrCreate(_cacheKeyOfPool, x => {
				x.SetSlidingExpiration(Timeout);
				return new ConcurrentDictionary<string, T>();
			});
		}
	}
}
