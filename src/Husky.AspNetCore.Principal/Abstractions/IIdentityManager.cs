namespace Husky.AspNetCore.Principal
{
	public interface IIdentityManager
	{
		IIdentity ReadIdentity();
		void SaveIdentity(IIdentity identity);
		void DeleteIdentity();
	}
}