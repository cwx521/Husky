namespace Husky.Principal
{
	public interface IIdentityManager
	{
		IIdentityOptions Options { get; }
		string? ReadRawToken();
		IIdentity ReadIdentity();
		void SaveIdentity(IIdentity identity);
		void DeleteIdentity();
	}
}