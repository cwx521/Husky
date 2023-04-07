using System;

namespace Husky.Principal
{
	public class PrincipalUser : Identity, IIdentity, IIdentityAnonymous, IPrincipalUser
	{
		internal PrincipalUser(IServiceProvider serviceProvider) {
			ServiceProvider = serviceProvider;
		}

		public PrincipalUser(IServiceProvider serviceProvider, IIdentityManager identityManager) : this(serviceProvider) {
			var read = identityManager.ReadIdentity();

			Id = read.Id;
			DisplayName = read.DisplayName;
			IsConsolidated = read.IsConsolidated;
			AnonymousId = read.AnonymousId;

			IdentityManager = identityManager;
			IdentityManager.SaveIdentity(read);
		}

		public IServiceProvider ServiceProvider { get; }
		public IIdentityManager? IdentityManager { get; }


		// Personate

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
	}
}