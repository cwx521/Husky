using System;

namespace Husky.AspNetCore
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
	public sealed class LabelAttribute : Attribute
	{
		public LabelAttribute(string label, string description = null) {
			Label = label;
			Description = description;
		}

		public string Label { get; set; }
		public string Description { get; set; }
	}
}