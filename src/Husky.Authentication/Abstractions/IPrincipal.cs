using System;
using Microsoft.AspNetCore.Http;

namespace Husky.Authentication.Abstractions
{
	public interface IPrincipal : IIdentity
	{
		IIdentityManager IdentityManager { get; }
		IServiceProvider ServiceProvider { get; }
		HttpContext HttpContext { get; }
	}
}