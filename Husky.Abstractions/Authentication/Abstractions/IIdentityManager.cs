namespace Husky.Authentication.Abstractions
{
	public interface IIdentityManager
	{
		IIdentity ReadIdentity();
		void SaveIdentity(IIdentity identity);
		void DeleteIdentity();
	}
}