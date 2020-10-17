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

			var iv = Crypto.SHA1(identity.Id + identity.DisplayName + identity.IsConsolidated + token);
			return Crypto.Encrypt($"{identity.Id}|{identity.DisplayName}|{identity.IsConsolidated}", iv, token) + iv;
		}

		public IIdentity? Decrypt(string encrypted, string token) {
			if ( encrypted == null ) {
				throw new ArgumentNullException(nameof(encrypted));
			}
			if ( token == null ) {
				throw new ArgumentNullException(nameof(token));
			}
			try {
				//iv is a Crypto.SHA1 result
				const int ivLength = 40;

				var iv = encrypted[^ivLength..];
				var str = Crypto.Decrypt(encrypted[..^ivLength], iv, token);
				var splitAt = str.IndexOf('|');
				var splitAtLast = str.LastIndexOf('|');

				return new Identity {
					Id = str[0..splitAt].AsInt(),
					DisplayName = str[(splitAt + 1)..splitAtLast],
					IsConsolidated = str[(splitAtLast + 1)..].AsBool()
				};
			}
			catch {
			}
			return null;
		}

		string IIdentityEncyptor.Encrypt(IIdentity identity, string token) => Encrypt(identity, token);
		IIdentity? IIdentityEncyptor.Decrypt(string encrypted, string token) => Decrypt(encrypted, token);
	}
}
