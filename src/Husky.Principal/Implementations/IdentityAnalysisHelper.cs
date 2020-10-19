namespace Husky.Principal.Implements
{
	internal static class IdentityHelper
	{
		internal const string AnonymousKey = "HUSKY_AUTH_ANONYMOUS";

		internal static IIdentity GetIdentity(string primaryStorage, string secondaryStorage, IdentityOptions options) {
			var identity = new Identity();
			identity.AnonymousId = secondaryStorage.AsGuid(identity.AnonymousId);

			if ( string.IsNullOrEmpty(primaryStorage) ) {
				return identity;
			}

			var decrypted = options.Encryptor.Decrypt(primaryStorage, options.Token);
			if ( decrypted == null ) {
				return identity;
			}

			decrypted.AnonymousId = identity.AnonymousId;
			return decrypted;
		}
	}
}
