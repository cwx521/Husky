using System;

namespace Husky.GridQuery
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class AppearanceAttribute : Attribute
	{
		public AppearanceAttribute() {
		}

		public AppearanceAttribute(string title) {
			Title = title;
		}

		public string? Title { get; set; }
		public int Width { get; set; }
		public TextAlign TextAlign { get; set; }

		public bool Visible { get; set; } = true;
		public bool Hidden { get; set; } = false;
		public bool Locked { get; set; } = false;

		public string? Format { get; set; }
		public string? Template { get; set; }
		public GridColumnTemplate KnownTemplate { get; set; }
		public string? CssClass { get; set; }
		public string? CssStyle { get; set; }
	}
}