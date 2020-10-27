using System;

namespace Husky.GridQuery
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public sealed class GridColumnRowNumberAttribute : GridColumnAttribute
	{
		public GridColumnRowNumberAttribute() {
			Width = 40;
			Filterable = false;
			Sortable = false;
			Locked = true;
			Title = " ";
			CssClass = "rownumber small text-right text-muted bg-light";
		}
	}
}