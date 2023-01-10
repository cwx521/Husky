using System;

namespace Husky.GridQuery
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public sealed class EnableRowNumberAttribute : Attribute
	{
		public EnableRowNumberAttribute() {
		}

		public int Width => 40;
		public bool Filterable => false;
		public bool Sortable => false;
		public bool Locked => true;
		public string Title => " ";
		public string CssClass => "row-number small text-end text-right text-muted bg-light";
	}
}