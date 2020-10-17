using System;

namespace Husky.Principal
{
	public class PrincipalUser : Identity, IIdentity, IIdentityAnonymous, IPrincipalUser
	{
		public PrincipalUser(IIdentityManager identityManager, IServiceProvider serviceProvider) {
			var identity = identityManager.ReadIdentity();

			AnonymousId = identity.AnonymousId;
			Id = identity.Id;
			DisplayName = identity.DisplayName;
			IsConsolidated = identity.IsConsolidated;

			ServiceProvider = serviceProvider;
			IdentityManager = identityManager;

			identityManager.SaveIdentity(identity);
		}

		public IServiceProvider ServiceProvider { get; }
		public IIdentityManager? IdentityManager { get; }


		public static PrincipalUser Personate(Identity identity, IServiceProvider serviceProvider) {
			return new PrincipalUser(serviceProvider) {
				Id = identity.Id,
				DisplayName = identity.DisplayName,
				IsConsolidated = false
			};
		}

		public static PrincipalUser Personate(int id, string displayName, IServiceProvider serviceProvider) {
			return new PrincipalUser(serviceProvider) {
				Id = id,
				DisplayName = displayName,
				IsConsolidated = false
			};
		}

		private PrincipalUser(IServiceProvider serviceProvider) {
			ServiceProvider = serviceProvider;
		}
	}
}