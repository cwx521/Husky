using System;

namespace Husky.GridQuery
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class CategoryAttribute : Attribute
	{
		public CategoryAttribute() {
		}

		public CategoryAttribute(string categoryName) {
			Category = categoryName;
		}

		public string Category { get; set; } = null!;
	}
}
