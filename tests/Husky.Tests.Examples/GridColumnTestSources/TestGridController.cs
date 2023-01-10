using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Husky.Tests.Examples
{
	[IgnoreAntiforgeryToken]
	public class TestGridController : Controller
	{
		public IActionResult TestGridDataRows() => new JsonResult(
			new {
				TotalCount = 3,
				Data = TestGridModel.BuildTestDataSource()
			},
			new JsonSerializerOptions {
				PropertyNamingPolicy = null
			}
		);
	}
}
