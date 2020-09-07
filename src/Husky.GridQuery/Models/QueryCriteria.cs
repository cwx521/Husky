using System.Collections.Generic;
using Husky;
using Newtonsoft.Json;

namespace Husky.GridQuery
{
	public class QueryCriteria
	{
		public bool PaginationEnabled = true;
		public int Limit { get; set; } = 100;
		public int Offset { get; set; }

		public string? SortBy { get; set; }
		public SortDirection SortDirection { get; set; }

		public List<QueryFilter> PreFilters { get; set; } = new List<QueryFilter>();
		public List<QueryFilter> PostFilters { get; set; } = new List<QueryFilter>();


		public string Json() => JsonConvert.SerializeObject(this, new JsonSerializerSettings {
			DefaultValueHandling = DefaultValueHandling.Ignore,
			NullValueHandling = NullValueHandling.Ignore
		});
		public override string ToString() => Json();
	}
}
