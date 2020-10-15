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

			var iv = Crypto.SHA1(identity.Id + identity.DisplayName + token);
			return Crypto.Encrypt($"{identity.Id}|{identity.DisplayName}", iv, token) + iv;
		}

		public IIdentity? Decrypt(string encryptedString, string token) {
			if ( encryptedString == null ) {
				throw new ArgumentNullException(nameof(encryptedString));
			}
			if ( token == null ) {
				throw new ArgumentNullException(nameof(token));
			}
			try {
				//iv is a Crypto.SHA1 result
				const int ivLength = 40;

				var iv = encryptedString[^ivLength..];
				var str = Crypto.Decrypt(encryptedString[..^ivLength], iv, token);
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
		IIdentity? IIdentityEncyptor.Decrypt(string encryptedString, string token) => Decrypt(encryptedString, token);
	}
}
