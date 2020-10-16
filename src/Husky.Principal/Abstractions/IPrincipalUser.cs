using System;

namespace Husky.Principal
{
	public interface IPrincipalUser : IIdentity
	{
		IServiceProvider ServiceProvider { get; }
		IIdentityManager? IdentityManager { get; }
	}
}