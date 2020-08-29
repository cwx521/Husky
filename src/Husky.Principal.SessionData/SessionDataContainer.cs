using System;
using System.Collections.Concurrent;

namespace Husky.Principal
{
	public sealed class SessionDataContainer : ConcurrentDictionary<string, object>, ISessionDataContainer
	{
		internal SessionDataContainer(IPrincipalUser principal) {
			_principal = principal;
		}

		private readonly IPrincipalUser _principal;

		string ISessionDataContainer.Key => _principal.IdString;
		DateTime ISessionDataContainer.ActiveTime { get; set; }
	}
}
