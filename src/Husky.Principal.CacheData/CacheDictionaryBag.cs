using System;
using System.Collections.Concurrent;

namespace Husky.Principal
{
	public sealed class CacheDictionaryBag : ConcurrentDictionary<string, object>, ICacheDataBag
	{
		internal CacheDictionaryBag(string key) {
			_key = key;
		}

		private readonly string _key;

		string ICacheDataBag.Key => _key;
		DateTime ICacheDataBag.ActiveTime { get; set; } = DateTime.Now;
	}
}
