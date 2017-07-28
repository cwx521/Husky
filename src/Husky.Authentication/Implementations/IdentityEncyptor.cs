using System;
using Husky.Authentication.Abstractions;
using Husky.Sugar;

namespace Husky.Authentication.Implementations
{
	public sealed class IdentityEncryptor<T> : IIdentityEncyptor<T> where T : IFormattable, IEquatable<T>
	{
		string IIdentityEncyptor<T>.Encrypt(Identity<T> identity, string token) {
			if ( identity == null ) {
				throw new ArgumentNullException(nameof(identity));
			}
			if ( token == null ) {
				throw new ArgumentNullException(nameof(token));
			}
			return Crypto.Encrypt($"{identity.Id}|{identity.DisplayName}|{Crypto.SHA1(identity.Id + identity.DisplayName + token)}", token);
		}

		Identity<T> IIdentityEncyptor<T>.Decrypt(string encryptedString, string token) {
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
				var validation = str.Substring(b + 1);
				var identity = new Identity<T> {
					Id = str.Substring(0, a).As<T>(),
					DisplayName = str.Substring(a + 1, b - a - 1)
				};
				if ( Crypto.SHA1(identity.Id + identity.DisplayName + token) != validation ) {
					return null;
				}
				return identity;
			}
			catch {
				return null;
			}
		}
	}
}
