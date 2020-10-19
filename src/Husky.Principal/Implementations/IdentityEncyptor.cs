using System;

namespace Husky.Principal.Implementations
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

			var str = identity.ToString()!;
			var iv = Crypto.SHA1(str + token);
			return Crypto.Encrypt(str, iv, token) + iv;
		}

		public IIdentity? Decrypt(string? encrypted, string token) {
			if ( token == null ) {
				throw new ArgumentNullException(nameof(token));
			}
			if ( encrypted != null ) {
				//iv is a Crypto.SHA1 result
				const int ivLength = 40;
				const int guidLength = 36;

				try {
					var iv = encrypted[^ivLength..];
					var decrypted = Crypto.Decrypt(encrypted[..^ivLength], iv, token);

					var anonymousId = decrypted[0..guidLength];
					var remained = decrypted[(guidLength + 1)..];

					var splitAt = remained.IndexOf('|');
					var splitAtLast = remained.LastIndexOf('|');

					return new Identity {
						AnonymousId = anonymousId.AsGuid(Guid.NewGuid()),
						Id = remained[0..splitAt].AsInt(),
						DisplayName = remained[(splitAt + 1)..splitAtLast],
						IsConsolidated = remained[(splitAtLast + 1)..].AsBool()
					};
				}
				catch { }
			}
			return null;
		}

		string IIdentityEncyptor.Encrypt(IIdentity identity, string token) => Encrypt(identity, token);
		IIdentity? IIdentityEncyptor.Decrypt(string encrypted, string token) => Decrypt(encrypted, token);
	}
}
