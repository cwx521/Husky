using System;
using System.Collections.Generic;
using System.Linq;
using Husky.KeyValues.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.KeyValues
{
	public class KeyValueManager
	{
		public KeyValueManager(IServiceProvider svc, IMemoryCache cache) {
			_svc = svc;
			_cache = cache;
		}

		private static readonly object _lock = new object();
		private readonly string _cacheKey = nameof(KeyValueManager) + nameof(Items);

		private readonly IServiceProvider _svc;
		private readonly IMemoryCache _cache;

		private List<KeyValue> Items => _cache.GetOrCreate(_cacheKey, entry => {
			using var scope = _svc.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<KeyValueDbContext>();
			return db.KeyValues.AsNoTracking().ToList();
		});
		private KeyValue Find(string key) => Items.Find(x => x.Key == key);

		public IEnumerable<string> AllKeys => Items.Select(x => x.Key);
		public bool Exists(string key) => Items.Any(x => x.Key == key);

		public string GetString(string key) => Find(key)?.Value;
		public T Get<T>(string key, T defaultValue = default(T)) => GetString(key).As(defaultValue);
		public T GetOrAdd<T>(string key, T defaultValueIfNotExist) => (T)GetOrAdd(key, defaultValueIfNotExist, typeof(T));

		public object GetOrAdd(string key, object defaultValueIfNotExist, Type defaultValueType) {
			if ( key == null ) {
				throw new ArgumentNullException(nameof(key));
			}

			var item = Find(key);
			if ( item != null ) {
				return Convert.ChangeType(item.Value, defaultValueType);
			}

			item = new KeyValue {
				Key = key,
				Value = defaultValueIfNotExist?.ToString()
			};
			lock ( _lock ) {
				Items.Add(item);
			}
			return defaultValueIfNotExist;
		}

		public void AddOrUpdate<T>(string key, T value) {
			if ( key == null ) {
				throw new ArgumentNullException(nameof(key));
			}

			var item = Find(key);
			if ( item == null ) {
				item = new KeyValue {
					Key = key,
					Value = value?.ToString()
				};
				lock ( _lock ) {
					Items.Add(item);
				}
			}
			else {
				lock ( _lock ) {
					item.Value = value?.ToString();
				}
			}
		}

		public void Reload() => _cache.Remove(_cacheKey);

		public void SaveChanges() {
			using var scope = _svc.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<KeyValueDbContext>();
			var fromDb = db.KeyValues.ToList();
			var added = Items.Where(x => !fromDb.Any(d => x.Key == d.Key)).ToList();

			fromDb.RemoveAll(x => !AllKeys.Contains(x.Key));
			fromDb.ForEach(x => x.Value = GetString(x.Key));
			db.AddRange(added);

			db.SaveChanges();
		}
	}
}