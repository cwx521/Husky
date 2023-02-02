using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Husky.GridQuery
{
	public class QueryResult<T> : IActionResult
	{
		public int totalCount { get; set; }
		public List<T> data { get; set; } = null!;

		public async Task ExecuteResultAsync(ActionContext context) => await new JsonResult(this, new JsonSerializerOptions(JsonSerializerDefaults.General)).ExecuteResultAsync(context);
	}
}
