using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;

namespace Husky.Principal
{
	internal class CacheDataPool<T> where T : class, ICacheDataContainer
	{
		internal CacheDataPool(IMemoryCache cache) {
			_cache = cache;
		}

		private readonly IMemoryCache _cache;
		private static readonly object _lock = new object();
		private static readonly string _cacheKey = "Pool_" + typeof(T).FullName;

		internal TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(30);

		internal T? Pick(string key) {
			if ( key == null ) {
				throw new ArgumentNullException(nameof(key));
			}
			var pool = GetPool();
			if ( pool != null ) {
				if ( pool.TryGetValue(key, out var bag) ) {
					if ( bag.ActiveTime.Add(Timeout) > DateTime.Now ) {
						lock ( _lock ) {
							bag.ActiveTime = DateTime.Now;
						}
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
			var pool = EnsureGetPool();
			lock ( _lock ) {
				pool[bag.Key] = bag;
			}
		}

		internal T PickOrCreate(string key, T whenNotExists) {
			if ( key == null ) {
				throw new ArgumentNullException(nameof(key));
			}
			if ( whenNotExists == null ) {
				throw new ArgumentNullException(nameof(whenNotExists));
			}
			if ( key != whenNotExists.Key ) {
				throw new ArgumentException($"The Key value of {nameof(whenNotExists)} must equal to the first given parameter.", nameof(whenNotExists));
			}

			var pool = EnsureGetPool();

			if ( pool.TryGetValue(key, out var bag) && bag.ActiveTime.Add(Timeout) > DateTime.Now ) {
				lock ( _lock ) {
					bag.ActiveTime = DateTime.Now;
				}
				return bag;
			}

			whenNotExists.ActiveTime = DateTime.Now;
			lock ( _lock ) {
				pool[key] = whenNotExists;
			}
			return whenNotExists;
		}

		internal void Drop(string key) {
			if ( key == null ) {
				throw new ArgumentNullException(nameof(key));
			}
			lock ( _lock ) {
				GetPool()?.Remove(key);
			}
		}

		internal void DropAll() => _cache.Remove(_cacheKey);

		internal void DropTimeout(int timeoutMinutes) => DropTimeout(TimeSpan.FromMinutes(timeoutMinutes));

		internal void DropTimeout(TimeSpan timeout) {
			var pool = GetPool();
			if ( pool != null ) {
				var keys = new string[pool.Count];
				pool.Keys.CopyTo(keys, 0);

				foreach ( var i in keys ) {
					if ( pool.ContainsKey(i) && pool[i].ActiveTime.Add(timeout) < DateTime.Now ) {
						lock ( _lock ) {
							pool.Remove(i);
						}
					}
				}
			}
		}

		private Dictionary<string, T> GetPool() => _cache.Get<Dictionary<string, T>>(_cacheKey);

		private Dictionary<string, T> EnsureGetPool() {
			return _cache.GetOrCreate(_cacheKey, x => {
				x.SetSlidingExpiration(Timeout);
				return new Dictionary<string, T>();
			});
		}
	}
}
