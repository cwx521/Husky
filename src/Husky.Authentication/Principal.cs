using System;
using Husky.Authentication.Abstractions;

namespace Husky.Authentication
{
	public abstract class Principal<T> : Identity<T>, IPrincipal<T> where T : IFormattable, IEquatable<T>
	{
		protected Principal(IIdentityManager<T> identityManager) {
			IdentityManager = identityManager;
			var identity = IdentityManager.ReadIdentity();

			if ( identity != null && identity.IsAuthenticated ) {
				Id = identity.Id;
				DisplayName = identity.DisplayName;
				IdentityManager.SaveIdentity(this);
			}
		}

		public IIdentityManager<T> IdentityManager { get; private set; }
	}
}