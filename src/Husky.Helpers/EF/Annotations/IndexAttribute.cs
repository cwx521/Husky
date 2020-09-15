using System;

namespace Husky
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public class IndexAttribute : Attribute
	{
		public bool IsUnique { get; set; } = true;

		public bool IsClustered { get; set; } = false;
	}
}
