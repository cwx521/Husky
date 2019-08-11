using System;

namespace Husky.GridQuery
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public class GridColumnCheckBoxAttribute : GridColumnAttribute
	{
		public GridColumnCheckBoxAttribute() {
			Width = 35;
			Filterable = false;
			Sortable = false;
			Title = $"<input type='checkbox' class='grid-header-checkbox' />";
			KnownTemplate = GridColumnTemplate.CheckBox;
		}
	}
}