using System.Linq;
using Husky.GridQuery;
using Microsoft.AspNetCore.Mvc;

namespace Husky.Tests.Examples
{
	[IgnoreAntiforgeryToken]
	public class TestGridController : Controller
	{
		public IActionResult TestGridDataRows(QueryCriteria criteria) => TestGridModel.BuildTestDataSource().AsQueryable().Apply(criteria);
	}
}
