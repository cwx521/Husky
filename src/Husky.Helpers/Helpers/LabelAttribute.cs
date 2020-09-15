using System;

namespace Husky
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
	public sealed class LabelAttribute : Attribute
	{
		public LabelAttribute(string label, string? description = null, string? cssClass = null) {
			Label = label;
			Description = description;
			CssClass = cssClass;
		}

		public string Label { get; set; }
		public string? Description { get; set; }
		public string? CssClass { get; set; }
	}
}