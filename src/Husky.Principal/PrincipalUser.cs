using System;

namespace Husky.Principal
{
	public class PrincipalUser : Identity, IIdentity, IIdentityAnonymous, IPrincipalUser
	{
		public PrincipalUser(IIdentityManager identityManager, IServiceProvider serviceProvider) {
			var identity = identityManager.ReadIdentity();

			Id = identity.Id;
			DisplayName = identity.DisplayName;
			IsConsolidated = identity.IsConsolidated;
			AnonymousId = identity.AnonymousId;

			ServiceProvider = serviceProvider;
			IdentityManager = identityManager;

			identityManager.SaveIdentity(identity);
		}

		public IServiceProvider ServiceProvider { get; }
		public IIdentityManager? IdentityManager { get; }


		public static IPrincipalUser Personate(Identity identity, IServiceProvider serviceProvider) {
			return new PrincipalUser(serviceProvider) {
				Id = identity.Id,
				DisplayName = identity.DisplayName
			};
		}

		public static IPrincipalUser Personate(int id, string displayName, IServiceProvider serviceProvider) {
			return new PrincipalUser(serviceProvider) {
				Id = id,
				DisplayName = displayName
			};
		}

		private PrincipalUser(IServiceProvider serviceProvider) {
			ServiceProvider = serviceProvider;
		}
	}
}