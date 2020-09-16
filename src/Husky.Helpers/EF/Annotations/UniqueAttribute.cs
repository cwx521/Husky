using System;

namespace Husky
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public class UniqueAttribute : Attribute
	{
	}
}
