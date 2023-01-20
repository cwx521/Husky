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

			for (var i = 1; i <= n; i++) {
				await Task.Run(() => {
					var key = "Test";
					var principal = PrincipalUser.Personate(i, null, serviceProvider);

					principal.Cache().GetOrAdd(key, _ => new Result {
						Ok = false,
						Message = i.ToString()
					});

					if (i % 111 == 0) {
						principal.AbandonCache();
					}
					else if (i % 5 == 0) {
						PrincipalUser.Personate(i, null, serviceProvider).Cache().TryGetValue(key, out var val);
						var data = (Result)val;
						data.Ok = true;
						data.Message = "Changed" + i;
					}
				});
			}

			for (var i = 1; i <= n; i++) {
				var principal = PrincipalUser.Personate(i, null, serviceProvider);
				var found = principal.Cache().TryGetValue("Test", out var val);

				if (i % 111 == 0) {
					Assert.IsFalse(found);
				}
				else {
					Assert.IsTrue(found);

					var data = (Result)val;
					if (i % 5 == 0) {
						Assert.AreEqual(true, data.Ok);
						Assert.AreEqual("Changed" + i, data.Message);
					}
					else {
						Assert.AreEqual(false, data.Ok);
						Assert.AreEqual(i.ToString(), data.Message);
					}
				}
			}
		}
	}
}