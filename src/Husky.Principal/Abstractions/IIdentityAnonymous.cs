using System;

namespace Husky.Principal
{
	public interface IIdentityAnonymous
	{
		Guid AnonymousId { get; set; }
	}
}
