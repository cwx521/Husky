using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Husky.KeyValues.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.KeyValues
{
	public class KeyValueManager : IKeyValueManager
	{
		public KeyValueManager(IServiceProvider services, IMemoryCache cache) {
			_services = services;
			_cache = cache;
		}

		private static readonly object _lock = new();
		private const int _cacheSlidingExpirationMinutes = 5;
		private const string _cacheKey = nameof(KeyValueManager);
		private readonly IServiceProvider _services;
		private readonly IMemoryCache _cache;

		public List<KeyValue> Items => _cache.GetOrCreate(_cacheKey, entry => {
			entry.SetSlidingExpiration(TimeSpan.FromMinutes(_cacheSlidingExpirationMinutes));
			var db = _services.CreateScope().ServiceProvider.GetRequiredService<IKeyValueDbContext>();
			return db.KeyValues.AsNoTracking().ToList();
		})!;

		public IEnumerable<string> AllKeys => Items.Select(x => x.Key);
		public bool Exists(string key) => Items.Any(x => x.Key == key);
		public KeyValue? Find(string key) => Items.Find(x => x.Key == key);

		public T Get<T>(string key, T fallback = default) where T : struct => Get(key).As<T>(fallback);
		public string? Get(string key) => Find(key)?.Value;

		public T GetOrAdd<T>(string key, T fallback) where T : struct => GetOrAdd(key, fallback.ToString()).As<T>();
		public string? GetOrAdd(string key, string? fallback) {
			if (key == null) {
				throw new ArgumentNullException(nameof(key));
			}

			var item = Find(key);
			if (item == null) {
				item = new KeyValue {
					Key = key,
					Value = fallback
				};
				lock (_lock) {
					Items.Add(item);
				}
			}
			return item.Value;
		}

		public void AddOrUpdate<T>(string key, T value) where T : struct => AddOrUpdate(key, value.ToString());
		public void AddOrUpdate(string key, string? value) {
			if (key == null) {
				throw new ArgumentNullException(nameof(key));
			}

			var item = Find(key);
			if (item == null) {
				item = new KeyValue {
					Key = key,
					Value = value
				};
				lock (_lock) {
					Items.Add(item);
				}
			}
			else {
				lock (_lock) {
					item.Value = value;
				}
			}
		}

		public void Reload() => _cache.Remove(_cacheKey);

		public void Save<T>(string key, T value) where T : struct => Save(key, value.ToString());
		public void Save(string key, string? value) {
			AddOrUpdate(key, value);

			using var scope = _services.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<IKeyValueDbContext>().Normalize();
			db.AddOrUpdate(new KeyValue {
				Key = key,
				Value = value
			});
			db.SaveChanges();
		}

		public void SaveAll() => SaveAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();

		public async Task SaveAllAsync() {
			using var scope = _services.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<IKeyValueDbContext>();
			var currentRows = db.KeyValues.ToList();
			var addingRows = Items.Where(x => !currentRows.Any(d => x.Key == d.Key)).ToList();

			currentRows.RemoveAll(x => !AllKeys.Contains(x.Key));
			currentRows.ForEach(x => x.Value = Get(x.Key));
			db.KeyValues.AddRange(addingRows);

			await db.Normalize().SaveChangesAsync();
		}
	}
}