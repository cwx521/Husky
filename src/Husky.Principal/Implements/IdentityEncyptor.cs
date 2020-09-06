using System;

namespace Husky.Principal.Implements
{
	public sealed class IdentityEncryptor : IIdentityEncyptor
	{
		public string Encrypt(IIdentity identity, string token) {
			if ( identity == null ) {
				throw new ArgumentNullException(nameof(identity));
			}
			if ( token == null ) {
				throw new ArgumentNullException(nameof(token));
			}

			var iv = Crypto.SHA1(identity.Id + identity.DisplayName);
			return Crypto.Encrypt($"{identity.Id}|{identity.DisplayName}", iv, token) + iv;
		}

		public IIdentity Decrypt(string encryptedString, string token) {
			if ( encryptedString == null ) {
				throw new ArgumentNullException(nameof(encryptedString));
			}
			if ( token == null ) {
				throw new ArgumentNullException(nameof(token));
			}
			try {
				var ivSaltStringLength = 40;
				var iv = encryptedString[^ivSaltStringLength..];
				var str = Crypto.Decrypt(encryptedString[..^ivSaltStringLength], iv, token);
				var splitAt = str.IndexOf('|');

				return new Identity {
					Id = str[0..splitAt].AsInt(),
					DisplayName = str[(splitAt + 1)..]
				};
			}
			catch {
			}
			return null;
		}

		string IIdentityEncyptor.Encrypt(IIdentity identity, string token) => Encrypt(identity, token);
		IIdentity IIdentityEncyptor.Decrypt(string encryptedString, string token) => Decrypt(encryptedString, token);
	}
}
