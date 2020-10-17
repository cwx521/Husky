using System;

namespace Husky.Principal
{
	public interface ICacheDataContainer
	{
		string Key { get; }
		DateTime ActiveTime { get; set; }
	}
}
