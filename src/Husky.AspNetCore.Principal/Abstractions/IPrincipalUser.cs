using System;

namespace Husky.AspNetCore.Principal
{
	public interface IPrincipalUser : IIdentity
	{
		IIdentityManager IdentityManager { get; }
		IServiceProvider ServiceProvider { get; }
	}
}