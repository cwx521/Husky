using System;
using System.Collections.Generic;
using System.Linq;
using Husky.KeyValues.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Husky.KeyValues
{
	public class KeyValueManager
	{
		protected KeyValueManager(KeyValueDbContext configurationDb, IMemoryCache cache) {
			_configurationDb = configurationDb;
			_cache = cache;
		}

		private static readonly object _lock = new object();
		private readonly string _cacheKey = nameof(KeyValueManager) + nameof(Items);

		private readonly KeyValueDbContext _configurationDb;
		private readonly IMemoryCache _cache;

		private List<KeyValue> Items => _cache.GetOrCreate(_cacheKey, entry => _configurationDb.KeyValues.AsNoTracking().ToList());
		private KeyValue Find(string key) => Items.Find(x => x.Key == key);

		public IEnumerable<string> AllKeys => Items.Select(x => x.Key);
		public bool Exists(string key) => Items.Any(x => x.Key == key);

		public string GetString(string key) => Find(key)?.Value;
		public T Get<T>(string key, T defaultValue = default(T)) => GetString(key).As(defaultValue);

		public T GetOrAdd<T>(string key, T defaultValueIfNotExist) {
			if ( key == null ) {
				throw new ArgumentNullException(nameof(key));
			}

			var item = Find(key);
			if ( item != null ) {
				return item.Value.As<T>();
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

		public void Update<T>(string key, T value) => Set(key, value, false);
		public void AddOrUpdate<T>(string key, T value) => Set(key, value, true);

		private void Set<T>(string key, T value, bool allowAdd) {
			if ( key == null ) {
				throw new ArgumentNullException(nameof(key));
			}

			var item = Find(key);
			if ( item == null ) {
				if ( !allowAdd ) {
					return;
				}
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

		public void Refresh() => _cache.Remove(_cacheKey);

		public void SaveChanges() {
			var fromDb = _configurationDb.KeyValues.ToList();
			var added = Items.Where(x => !fromDb.Any(d => x.Key == d.Key)).ToList();

			fromDb.RemoveAll(x => !AllKeys.Contains(x.Key));
			fromDb.ForEach(x => x.Value = GetString(x.Key));
			fromDb.AddRange(added);

			_configurationDb.SaveChanges();
		}
	}
}