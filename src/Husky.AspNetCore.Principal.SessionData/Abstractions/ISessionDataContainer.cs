using System;

namespace Husky.AspNetCore.Principal
{
	public interface ISessionDataContainer
	{
		string Key { get; }
		DateTime ActiveTime { get; set; }
	}
}
