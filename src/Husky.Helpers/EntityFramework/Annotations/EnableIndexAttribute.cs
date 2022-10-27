using System;

namespace Husky
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public sealed class EnableIndexAttribute : Attribute
	{
		public bool IsUnique { get; init; }
		public bool IsClustered { get; init; }
	}
}
