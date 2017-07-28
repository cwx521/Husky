using System;

namespace Husky.Authentication.Abstractions
{
	public interface IIdentityEncyptor
	{
		string Encrypt(IIdentity identity, string token);
		IIdentity Decrypt(string encryptedString, string token);
	}
}