using System;
using System.Collections.Concurrent;
using Husky.Authentication.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;


namespace Husky.PrincipalBaggage
{
	public sealed class Baggage : ConcurrentDictionary<string, object>, IBaggage
	{
		internal Baggage(IPrincipal principal) {
			_principal = principal;
		}

		IPrincipal _principal;

		public void Refresh() {
			var cache = _principal.ServiceProvider.GetService<IMemoryCache>();
			new BaggagePool<Baggage>(cache).Drop(_principal.IdString);
		}

		string IBaggage.Key => _principal.IdString;
		DateTime IBaggage.ActiveTime { get; set; }
	}
}
