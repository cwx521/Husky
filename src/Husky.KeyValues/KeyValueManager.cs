using System;
using System.Collections.Generic;
using System.Linq;
using Husky.KeyValues.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Husky.KeyValues
{
	public class KeyValueManager : IKeyValueManager
	{
		public KeyValueManager(IKeyValueDbContext db, IMemoryCache cache) {
			_db = db;
			_cache = cache;
		}

		private static readonly object _lock = new object();
		private readonly string _cacheKey = nameof(KeyValueManager) + nameof(Items);

		private readonly IKeyValueDbContext _db;
		private readonly IMemoryCache _cache;

		public IEnumerable<string> AllKeys => Items.Select(x => x.Key);
		public bool Exists(string key) => Items.Any(x => x.Key == key);

		public List<KeyValue> Items => _cache.GetOrCreate(_cacheKey, entry => _db.KeyValues.AsNoTracking().ToList());
		public KeyValue? Find(string key) => Items.Find(x => x.Key == key);

		public string? Get(string key) => Find(key)?.Value;
		public T Get<T>(string key, T defaultValue = default) where T : struct => Get(key).As(defaultValue);

		public T GetOrAdd<T>(string key, T defaultValueIfNotExist) where T : struct => GetOrAdd(key, defaultValueIfNotExist.ToString()).As<T>();
		public string? GetOrAdd(string key, string? defaultValueIfNotExist) {
			if ( key == null ) {
				throw new ArgumentNullException(nameof(key));
			}

			var item = Find(key);
			if ( item != null ) {
				return item.Value;
			}

			item = new KeyValue {
				Key = key,
				Value = defaultValueIfNotExist
			};
			lock ( _lock ) {
				Items.Add(item);
			}
			return defaultValueIfNotExist;
		}

		public void AddOrUpdate<T>(string key, T value) where T : struct => AddOrUpdate(key, value.ToString());
		public void AddOrUpdate(string key, string? value) {
			if ( key == null ) {
				throw new ArgumentNullException(nameof(key));
			}

			var item = Find(key);
			if ( item == null ) {
				item = new KeyValue {
					Key = key,
					Value = value
				};
				lock ( _lock ) {
					Items.Add(item);
				}
			}
			else {
				lock ( _lock ) {
					item.Value = value;
				}
			}
		}

		public void Reload() => _cache.Remove(_cacheKey);

		public void Save<T>(string key, T value) where T : struct => Save(key, value.ToString());
		public void Save(string key, string? value) {
			AddOrUpdate(key, value);

			_db.Normalize().AddOrUpdate(new KeyValue {
				Key = key,
				Value = value
			});
			_db.Normalize().SaveChanges();
		}

		public void SaveAll() {
			var fromDb = _db.KeyValues.ToList();
			var added = Items.Where(x => !fromDb.Any(d => x.Key == d.Key)).ToList();

			fromDb.RemoveAll(x => !AllKeys.Contains(x.Key));
			fromDb.ForEach(x => x.Value = Get(x.Key));
			_db.KeyValues.AddRange(added);

			_db.Normalize().SaveChanges();
		}
	}
}