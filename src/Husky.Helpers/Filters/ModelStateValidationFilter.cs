using Microsoft.AspNetCore.Mvc.Filters;

namespace Husky
{
	public class ModelStateValidationFilter : IPageFilter, IActionFilter
	{
		public void OnPageHandlerExecuting(PageHandlerExecutingContext context) {
			if ( !context.ModelState.IsValid ) {
				context.Result = context.ModelState.ToResult();
			}
		}

		public void OnActionExecuting(ActionExecutingContext context) {
			if ( !context.ModelState.IsValid ) {
				context.Result = context.ModelState.ToResult();
			}
		}

		public void OnPageHandlerSelected(PageHandlerSelectedContext context) { }
		public void OnPageHandlerExecuted(PageHandlerExecutedContext context) { }
		public void OnActionExecuted(ActionExecutedContext context) { }
	}
}
