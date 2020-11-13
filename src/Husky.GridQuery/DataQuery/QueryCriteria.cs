using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Husky.GridQuery
{
	public class QueryCriteria
	{
		public bool PaginationEnabled { get; set; } = true;
		public int Limit { get; set; } = 100;
		public int Offset { get; set; }

		public string? SortBy { get; set; }
		public SortDirection SortDirection { get; set; }

		public List<QueryFilter> PreFilters { get; set; } = new List<QueryFilter>();
		public List<QueryFilter> PostFilters { get; set; } = new List<QueryFilter>();


		public string Json() => JsonSerializer.Serialize(this, new JsonSerializerOptions(JsonSerializerDefaults.Web) {
			DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
		});

		public override string ToString() => Json();
	}
}
