using System.Linq;
using System.Threading.Tasks;
using Husky.GridQuery;
using Microsoft.AspNetCore.Mvc;

namespace Husky.Tests.Examples
{
	[IgnoreAntiforgeryToken]
	public class TestGridController : Controller
	{
		public IActionResult TestGridDataRows(QueryCriteria criteria) {
			return TestGridModel.BuildTestDataSource().AsQueryable().ToResult(criteria);
		}
	}
}
