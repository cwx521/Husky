namespace Husky.Principal
{
	public interface IIdentityEncyptor
	{
		string Encrypt(IIdentity identity, string token);
		IIdentity Decrypt(string encryptedString, string token);
	}
}