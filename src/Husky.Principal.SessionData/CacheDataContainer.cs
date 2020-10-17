using System;
using System.Collections.Concurrent;

namespace Husky.Principal
{
	public sealed class CacheDataContainer : ConcurrentDictionary<string, object>, ICacheDataContainer
	{
		internal CacheDataContainer(IPrincipalUser principal) {
			_principal = principal;
		}

		private readonly IPrincipalUser _principal;

		string ICacheDataContainer.Key => _principal.CacheKey();
		DateTime ICacheDataContainer.ActiveTime { get; set; }
	}
}
