using System;

namespace Husky.Principal
{
	public class PrincipalUser : Identity, IIdentity, IIdentityAnonymous, IPrincipalUser
	{
		private PrincipalUser(IServiceProvider serviceProvider) {
			ServiceProvider = serviceProvider;
		}

		public PrincipalUser(IServiceProvider serviceProvider, IIdentityManager identityManager) : this(serviceProvider) {
			IdentityManager = identityManager;
			var options = IdentityManager.Options;
			var read = IdentityManager.ReadIdentity();

			Id = read.Id;
			DisplayName = read.DisplayName;
			IsConsolidated = read.IsConsolidated;
			AnonymousId = read.AnonymousId;

			if (IsAuthenticated && options.Expires.HasValue && (options.Carrier == IdentityCarrier.Cookie || options.Carrier == IdentityCarrier.HeaderAndCookie)) {
				IdentityManager.SaveIdentity(read);
			}
		}

		public IServiceProvider ServiceProvider { get; }
		public IIdentityManager? IdentityManager { get; }

		public string? GenerateIdentityToken() {
			return IsAnonymous || IdentityManager == null
				? null
				: IdentityManager.Options.Encryptor.Encrypt(this, IdentityManager.Options.Token);
		}


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