using System;

namespace Husky.Principal
{
	public interface ICacheDataBag
	{
		string Key { get; }
		DateTime ActiveTime { get; set; }
	}
}
