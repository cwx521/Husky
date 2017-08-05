using System;
using Husky.Authentication.Abstractions;

namespace Husky.Authentication
{
	public class Principal<T> : Identity, IIdentity, IPrincipal where T : struct, IFormattable, IEquatable<T>
	{
		public Principal(IIdentityManager identityManager, IServiceProvider serviceProvider) {

			var identity = identityManager.ReadIdentity();
			if ( identity != null && identity.IsAuthenticated ) {
				IdString = identity.IdString;
				DisplayName = identity.DisplayName;
				identityManager.SaveIdentity(this);
			}

			this.IdentityManager = identityManager;
			this.ServiceProvider = serviceProvider;
		}

		public T? Id {
			get => Id<T>();
			set => IdString = value?.ToString();
		}

		public IIdentityManager IdentityManager { get; private set; }
		public IServiceProvider ServiceProvider { get; private set; }
	}
}