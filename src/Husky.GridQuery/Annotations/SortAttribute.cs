using System;

namespace Husky.GridQuery
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public sealed class SortAttribute : Attribute
	{
		public SortAttribute() {
		}

		public SortAttribute(SortDirection sortDirection) {
			SortDirection = sortDirection;
		}

		public SortDirection SortDirection { get; set; }
	}
}