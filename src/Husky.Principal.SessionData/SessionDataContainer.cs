using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.Principal
{
	public sealed class SessionDataContainer : ConcurrentDictionary<string, object>, ISessionDataContainer
	{
		internal SessionDataContainer(IPrincipalUser principal) {
			_principal = principal;
		}

		IPrincipalUser _principal;

		string ISessionDataContainer.Key => _principal.IdString;
		DateTime ISessionDataContainer.ActiveTime { get; set; }
	}
}
