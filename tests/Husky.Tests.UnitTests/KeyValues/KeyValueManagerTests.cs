using System.Threading.Tasks;
using Husky.KeyValues.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Husky.KeyValues.Tests
{
	[TestClass()]
	public class KeyValueManagerTests
	{
		[TestMethod()]
		public async Task MarshalTestAsync() {
			var serviceProvider = new ServiceCollection()
				.AddDbContext<IKeyValueDbContext, KeyValueDbContext>(x => x.UseInMemoryDatabase("UnitTest"))
				.BuildServiceProvider();

			var keyValues = new KeyValueManager(
				serviceProvider,
				new MemoryCache(new MemoryCacheOptions())
			);

			var key = "key";
			var value = "123";
			var value2 = "456";

			Assert.IsFalse(keyValues.Exists(key));
			Assert.IsNull(keyValues.Get(key));
			Assert.IsNull(keyValues.Find(key));
			Assert.AreEqual(0, keyValues.Get<int>(key));


			keyValues.AddOrUpdate(key, value);
			Assert.IsTrue(keyValues.Exists(key));
			Assert.IsNotNull(keyValues.Get(key));
			Assert.IsNotNull(keyValues.Find(key));
			Assert.AreEqual(value, keyValues.Get(key));
			Assert.AreEqual(value, keyValues.Find(key).Value);
			Assert.AreEqual(value, keyValues.GetOrAdd(key, Crypto.RandomString()));
			Assert.AreEqual(value.AsInt(), keyValues.Get<int>(key));

			keyValues.AddOrUpdate(key, value2.AsInt());
			Assert.IsTrue(keyValues.Exists(key));
			Assert.IsNotNull(keyValues.Get(key));
			Assert.IsNotNull(keyValues.Find(key));
			Assert.AreEqual(value2, keyValues.Get(key));
			Assert.AreEqual(value2, keyValues.Find(key).Value);
			Assert.AreEqual(value2.AsInt(), keyValues.Get<int>(key));

			keyValues.Reload();
			Assert.IsFalse(keyValues.Exists(key));
			Assert.IsNull(keyValues.Get(key));
			Assert.IsNull(keyValues.Find(key));

			keyValues.AddOrUpdate(key, value);
			keyValues.SaveAll();
			keyValues.Reload();
			Assert.IsTrue(keyValues.Exists(key));
			Assert.IsNotNull(keyValues.Get(key));
			Assert.IsNotNull(keyValues.Find(key));

			keyValues.AddOrUpdate(key, value2);
			await keyValues.SaveAllAsync();
			Assert.AreEqual(value2, keyValues.Get(key));

			using var db = serviceProvider.GetRequiredService<IKeyValueDbContext>();
			var row = db.KeyValues.Find(key);
			Assert.IsNotNull(row);
			Assert.AreEqual(key, row.Key);
			Assert.AreEqual(value2, row.Value);

			row.Value = Crypto.RandomString();
			db.Normalize().SaveChanges();
			keyValues.Reload();
			Assert.IsTrue(keyValues.Exists(key));
			Assert.AreEqual(row.Value, keyValues.Get(key));

			db.Normalize().Database.EnsureDeleted();
		}
	}
}