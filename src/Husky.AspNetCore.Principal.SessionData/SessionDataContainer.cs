using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.AspNetCore.Principal
{
	public sealed class SessionDataContainer : ConcurrentDictionary<string, object>, ISessionDataContainer
	{
		internal SessionDataContainer(IPrincipalUser principal) {
			_principal = principal;
		}

		IPrincipalUser _principal;

		public void ClearAll() {
			var cache = _principal.ServiceProvider.GetRequiredService<IMemoryCache>();
			new SessionDataPool<SessionDataContainer>(cache).Drop(_principal.IdString);
		}

		string ISessionDataContainer.Key => _principal.IdString;
		DateTime ISessionDataContainer.ActiveTime { get; set; }
	}
}
