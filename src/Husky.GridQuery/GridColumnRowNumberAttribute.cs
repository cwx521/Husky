using System;

namespace Husky.GridQuery
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public class GridColumnRowNumberAttribute : GridColumnAttribute
	{
		public GridColumnRowNumberAttribute() {
			Width = 40;
			Filterable = false;
			Sortable = false;
			Locked = true;
			Title = " ";
			CssClass = "rownumber text-muted-lighter text-11px bg-light text-right";
		}
	}
}