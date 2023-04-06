namespace Husky.Principal.Implementations
{
	internal static class IdentityReader
	{
		internal static IIdentity GetIdentity(string? primaryStorage, string? secondaryStorage, IIdentityOptions options) {
			var identity = string.IsNullOrEmpty(primaryStorage)
				? null
				: options.Encryptor.Decrypt(primaryStorage, options.Salt);

			identity ??= new Identity();

			if (options.DedicateAnonymousIdStorage && secondaryStorage != null) {
				identity.AnonymousId = secondaryStorage.AsGuid(identity.AnonymousId);
			}

			return identity;
		}
	}
}
