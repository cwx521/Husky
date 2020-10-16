using System;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.Principal
{
	public class PrincipalUser : Identity, IIdentity, IPrincipalUser
	{
		public PrincipalUser(IIdentityManager identityManager, IServiceProvider serviceProvider) {
			var identity = identityManager.ReadIdentity();

			if ( identity != null ) {
				Id = identity.Id;
				DisplayName = identity.DisplayName;
				IsConsolidated = identity.IsConsolidated;

				identityManager.SaveIdentity(identity);
			}

			ServiceProvider = serviceProvider;
			IdentityManager = identityManager;
		}

		private PrincipalUser(IServiceProvider serviceProvider) {
			ServiceProvider = serviceProvider;
			IdentityManager = serviceProvider.GetRequiredService<IIdentityManager>();
		}

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

		public IIdentityManager IdentityManager { get; }
		public IServiceProvider ServiceProvider { get; }
	}
}