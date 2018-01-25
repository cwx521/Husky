using System;

namespace Husky.AspNetCore.Principal.Abstractions
{
	public interface ISessionDataContainer
	{
		string Key { get; }
		DateTime ActiveTime { get; set; }
	}
}
