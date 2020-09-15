using System;

namespace Husky
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public class CompositeUniqueAttribute : Attribute
	{
		public bool IsUnique { get; set; }

		public bool Composite { get; set; }
	}
}
