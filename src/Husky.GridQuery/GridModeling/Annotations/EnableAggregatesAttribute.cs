using System;

namespace Husky.GridQuery.GridModeling.Annotations
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class EnableAggregatesAttribute : Attribute
	{
		public EnableAggregatesAttribute() {
		}

		public EnableAggregatesAttribute(GridColumnAggregates aggregates) {
			Aggregates = aggregates;
		}

		public virtual GridColumnAggregates Aggregates { get; set; }
	}
}
