using System;

namespace Husky.GridQuery
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class BehaviorAttribute : Attribute
	{
		public BehaviorAttribute() {
		}

		public bool Filterable { get; set; } = true;
		public bool Sortable { get; set; } = true;
		public bool Hidable { get; set; } = true;
		public bool Groupable { get; set; } = false;
		public bool Editable { get; set; } = false;
	}
}