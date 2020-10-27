using System;

namespace Husky.GridQuery
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class GridColumnCheckBoxAttribute : GridColumnAttribute
	{
		public GridColumnCheckBoxAttribute(bool showHeaderCheckBox = true) {
			Width = 35;
			Filterable = false;
			Sortable = false;
			Title = showHeaderCheckBox ? "<input type='checkbox' class='grid-header-checkbox' />" : " ";
			KnownTemplate = GridColumnTemplate.CheckBox;
		}
	}
}