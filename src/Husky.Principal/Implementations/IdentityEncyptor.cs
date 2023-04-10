using System;

namespace Husky.Principal.Implementations
{
	public sealed class IdentityEncryptor : IIdentityEncyptor
	{
		public string Encrypt(IIdentity identity, string token) {
			if (identity == null) {
				throw new ArgumentNullException(nameof(identity));
			}
			if (token == null) {
				throw new ArgumentNullException(nameof(token));
			}

			var str = identity.ToString()!;
			var iv = (str + token).SHA1();
			return Crypto.Encrypt(str, iv, token) + iv;
		}

		public IIdentity? Decrypt(string? encrypted, string token) {
			if (token == null) {
				throw new ArgumentNullException(nameof(token));
			}
			if (encrypted != null) {
				const int ivLength = 40;
				const int guidLength = 36;

				try {
					var iv = encrypted[^ivLength..];
					var decrypted = Crypto.Decrypt(encrypted[..^ivLength], iv, token);
					var anonymousId = decrypted[^guidLength..];
					var remained = decrypted[0..(decrypted.Length - guidLength - 1)];
					var arr = remained.Split('|');

					return new Identity {
						AnonymousId = anonymousId.AsGuid(Guid.NewGuid()),
						Id = arr[0].AsInt(),
						DisplayName = arr[1],
						IsConsolidated = arr[2].AsBool()
					};
				}
				catch { }
			}
			return null;
		}
	}
}
