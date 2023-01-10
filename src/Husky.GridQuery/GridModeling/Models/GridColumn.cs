using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Husky.GridQuery
{
	/// <summary>
	/// !!!
	/// The field names in this model should match the Kendo Grid definitions in javascript
	/// </summary>
	public sealed class GridColumn
	{
		public string? Field { get; set; }
		public string? Title { get; set; }
		public string? Category { get; set; }
		public string? Type { get; set; }
		public string[]? Aggregates { get; set; }
		public int? Width { get; set; } = GridColumnBuilder.DefaultGridColumnWidth;
		public string? Format { get; set; }
		public string? Template { get; set; }
		public bool Groupable { get; set; }
		public bool Filterable { get; set; }
		public bool Sortable { get; set; }
		public bool Hidable { get; set; }
		public bool Locked { get; set; }
		public bool Hidden { get; set; }
		public bool EditableFlag { get; set; }
		public SelectListItem[]? Values { get; set; }
		public List<GridColumn>? Columns { get; set; }
		public GridColumnTdAttributes? Attributes { get; set; }

	}
}