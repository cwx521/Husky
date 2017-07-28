using System;

namespace Husky.Authentication.Abstractions
{
	public interface IIdentityEncyptor
	{
		string Encrypt(Identity identity, string token);
		Identity Decrypt(string encryptedString, string token);
	}
}