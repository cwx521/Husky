using System.Collections.Generic;

namespace Husky.GridQuery
{
	/// <summary>
	/// !!!
	/// The field names in this model should match the Kendo Grid definitions in javascript
	/// </summary>
	public sealed class GridColumnSpec
	{
		public List<GridColumnSpec>? columns { get; set; }

		public string? field { get; set; }
		public string? title { get; set; }
		public string? type { get; set; }
		public string? group { get; set; }
		public int? width { get; set; } = 160;
		public string? attributes { get; set; }
		public string? format { get; set; }
		public string? template { get; set; }
		public bool filterable { get; set; }
		public bool sortable { get; set; }
		public bool hidable { get; set; }
		public bool locked { get; set; }
		public bool hidden { get; set; }
		public bool editableFlag { get; set; }
		public GridColumnSpecEnumItem[]? values { get; set; }

	}

	public sealed class GridColumnSpecEnumItem
	{
		public string text { get; set; } = null!;
		public int value { get; set; }
	}
}