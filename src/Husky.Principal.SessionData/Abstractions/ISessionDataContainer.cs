using System;

namespace Husky.Principal
{
	public interface ISessionDataContainer
	{
		string Key { get; }
		DateTime ActiveTime { get; set; }
	}
}
