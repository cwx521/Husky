using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Husky.GridQuery
{
	public class QueryResult<T> : Result<List<T>>, IActionResult
	{
		public int TotalCount { get; set; }
	}

	public class SuccessQueryResult<T> : QueryResult<T>
	{
		public SuccessQueryResult() {
			Ok = true;
		}
	}
}
