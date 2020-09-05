using System;

namespace Husky.Principal
{
	public class PrincipalUser : Identity, IIdentity, IPrincipalUser
	{
		public PrincipalUser(IIdentityManager identityManager, IServiceProvider serviceProvider) {

			var identity = identityManager.ReadIdentity();
			if ( identity != null && identity.IsAuthenticated ) {
				Id = identity.Id;
				DisplayName = identity.DisplayName;
				identityManager.SaveIdentity(this);
			}

			IdentityManager = identityManager;
			ServiceProvider = serviceProvider;
		}

		public IIdentityManager IdentityManager { get; private set; }
		public IServiceProvider ServiceProvider { get; private set; }
	}
}