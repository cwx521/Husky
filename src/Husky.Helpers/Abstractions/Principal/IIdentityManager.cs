using Microsoft.AspNetCore.Http;

namespace Husky.Principal
{
	public interface IIdentityManager
	{
		string? ReadRawToken();
		IIdentity ReadIdentity();
		void SaveIdentity(IIdentity identity);
		void DeleteIdentity();
	}
}