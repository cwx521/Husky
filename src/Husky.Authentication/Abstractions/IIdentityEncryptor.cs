using System;

namespace Husky.Authentication.Abstractions
{
	public interface IIdentityEncyptor<T> where T : IFormattable, IEquatable<T>
	{
		string Encrypt(Identity<T> identity, string token);
		Identity<T> Decrypt(string encryptedString, string token);
	}
}