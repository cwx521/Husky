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
			return Crypto.Encrypt($"{identity.Id}|{identity.DisplayName}|{Crypto.SHA1(identity.Id + identity.DisplayName + token)}", token);
		}

		public IIdentity Decrypt(string encryptedString, string token) {
			if ( encryptedString == null ) {
				throw new ArgumentNullException(nameof(encryptedString));
			}
			if ( token == null ) {
				throw new ArgumentNullException(nameof(token));
			}
			try {
				var str = Crypto.Decrypt(encryptedString, token);
				var a = str.IndexOf('|');
				var b = str.LastIndexOf('|');

				if ( a != -1 && b > a ) {
					var validation = str.Substring(b + 1);
					var identity = new Identity {
						Id = str.Substring(0, a).AsInt(),
						DisplayName = str.Substring(a + 1, b - a - 1)
					};
					if ( Crypto.SHA1(identity.Id + identity.DisplayName + token) == validation ) {
						return identity;
					}
				}
			}
			catch {
			}
			return null;
		}

		string IIdentityEncyptor.Encrypt(IIdentity identity, string token) => Encrypt(identity, token);
		IIdentity IIdentityEncyptor.Decrypt(string encryptedString, string token) => Decrypt(encryptedString, token);
	}
}
