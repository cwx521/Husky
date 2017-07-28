using System;

namespace Husky.Data.Modeling
{
	[Flags]
	public enum RowStatus
	{
		Active = 0,
		Pending = 1,
		Suspended = 1 << 1,
		Deleted = 1 << 2,
		BySelf = 1 << 30,
		ByAdmin = 1 << 31
	}
}