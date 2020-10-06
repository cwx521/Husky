using System;

namespace Husky
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public sealed class UniqueAttribute : Attribute
	{
	}
}
