using System;

namespace Husky.Authentication.Abstractions
{
	public interface IBaggage
	{
		string Key { get; }
		DateTime ActiveTime { get; set; }
	}
}
