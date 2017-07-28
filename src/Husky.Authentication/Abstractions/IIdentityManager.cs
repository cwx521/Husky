using System;

namespace Husky.Authentication.Abstractions
{
	public interface IIdentityManager
	{
		Identity ReadIdentity();
		void SaveIdentity(Identity identity);
		void DeleteIdentity();
	}
}