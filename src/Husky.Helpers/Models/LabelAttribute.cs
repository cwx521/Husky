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

		public string Label { get; init; }
		public string? Description { get; init; }
		public string? CssClass { get; init; }
	}
}