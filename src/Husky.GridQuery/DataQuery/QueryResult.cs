using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Husky.GridQuery
{
	public class QueryResult<T> : IActionResult
	{
		public int TotalCount { get; set; }
		public List<T> Data { get; set; } = null!;

		public async Task ExecuteResultAsync(ActionContext context) => await new JsonResult(this).ExecuteResultAsync(context);
	}
}
