using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Husky.Principal.Tests
{
	[TestClass()]
	public class PrincipalExtensionsTests
	{
		[TestMethod()]
		public async Task CacheDataTest() {
			var n = 10000;

			var services = new ServiceCollection();
			services.AddSingleton<IMemoryCache, MemoryCache>(x => new MemoryCache(new MemoryCacheOptions()));

			var serviceProvider = services.BuildServiceProvider();

			for ( var i = 1; i <= n; i++ ) {
				await Task.Run(() => {
					var principal = PrincipalUser.Personate(i, null, serviceProvider);
					principal.CacheData().GetOrAdd("Test", _ => new Result { Ok = false, Message = Crypto.SHA256(i.ToString()) });
					if ( i % 11 == 0 ) {
						principal.AbandonCache();
					}
					else if ( i % 5 == 0 ) {
						PrincipalUser.Personate(1, null, serviceProvider).CacheData().TryGetValue("Test", out var val);
						var data = (Result)val;
						data.Ok = true;
						data.Message = "Changed" + i;
					}
				});
			}

			for ( var i = 1; i <= n; i++ ) {
				var principal = PrincipalUser.Personate(i, null, serviceProvider);
				var found = principal.CacheData().TryGetValue("Test", out var val);

				if ( i % 11 == 0 ) {
					Assert.IsFalse(found);
				}
				else {
					Assert.IsTrue(found);

					var data = (Result)val;
					if ( i != 1 ) {
						Assert.AreEqual(false, data.Ok);
						Assert.AreEqual(Crypto.SHA256(i.ToString()), data.Message);
					}
					else {
						Assert.AreEqual(true, data.Ok);
						Assert.AreEqual("Changed" + n, data.Message);
					}
				}
			}
		}
	}
}