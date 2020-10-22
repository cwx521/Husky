using System;
using System.Collections.Concurrent;

namespace Husky.Principal
{
	public sealed class CacheDictionaryBag : ConcurrentDictionary<string, object>, ICacheDataBag
	{
		internal CacheDictionaryBag(IPrincipalUser principal) {
			_principal = principal;
		}

		private readonly IPrincipalUser _principal;

		string ICacheDataBag.Key => _principal.CacheKey();
		DateTime ICacheDataBag.ActiveTime { get; set; }
	}
}
