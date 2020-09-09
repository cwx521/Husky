using System.Collections.Generic;

namespace Husky.GridQuery
{
	/// <summary>
	/// !!!
	/// The field names in this model should match the Kendo Grid definitions in javascript
	/// </summary>
	public sealed class GridColumnSpec
	{
		public string? Field { get; set; }
		public string? Title { get; set; }
		public string? Type { get; set; }
		public string[]? Aggregates { get; set; }
		public string? Gather { get; set; }
		public int? Width { get; set; } = 160;
		public string? Attributes { get; set; }
		public string? Format { get; set; }
		public string? Template { get; set; }
		public bool Groupable { get; set; }
		public bool Filterable { get; set; }
		public bool Sortable { get; set; }
		public bool Hidable { get; set; }
		public bool Locked { get; set; }
		public bool Hidden { get; set; }
		public bool EditableFlag { get; set; }
		public GridColumnSpecEnumItem[]? Values { get; set; }
		public List<GridColumnSpec>? Columns { get; set; }

	}

	public sealed class GridColumnSpecEnumItem
	{
		public string Text { get; set; } = null!;
		public int Value { get; set; }
	}
}