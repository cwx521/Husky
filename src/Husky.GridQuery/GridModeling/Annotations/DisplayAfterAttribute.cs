using System;

namespace Husky.GridQuery
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class DisplayAfterAttribute : Attribute
	{
		public DisplayAfterAttribute() {
		}

		public DisplayAfterAttribute(string displayAfterColumnName) {
			DisplayAfter = displayAfterColumnName;
		}

		public string DisplayAfter { get; set; } = null!;
	}
}
