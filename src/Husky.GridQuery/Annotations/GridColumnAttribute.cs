using System;

namespace Husky.GridQuery
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public class GridColumnAttribute : Attribute
	{
		public virtual bool Visible { get; set; } = true;

		public virtual string Title { get; set; }
		public virtual int Width { get; set; }

		public virtual bool Filterable { get; set; } = true;
		public virtual bool Sortable { get; set; } = true;
		public virtual bool Hidable { get; set; } = true;
		public virtual bool Editable { get; set; } = false;
		public virtual bool Hidden { get; set; } = false;
		public virtual bool Locked { get; set; } = false;

		public virtual string CssClass { get; set; }
		public virtual string Url { get; set; }
		public virtual string Format { get; set; }
		public virtual string Template { get; set; }
		public virtual GridColumnTemplate KnownTemplate { get; set; }

		public virtual string Group { get; set; }
		public virtual string DisplayAfter { get; set; }
	}
}