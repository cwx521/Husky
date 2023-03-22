using System;

namespace Husky.GridQuery
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class EnableAggregatesAttribute : Attribute
	{
		public EnableAggregatesAttribute() {
		}

		public EnableAggregatesAttribute(GridColumnAggregates aggregates) {
			Aggregates = aggregates;
		}

		public GridColumnAggregates Aggregates { get; set; }
	}
}
