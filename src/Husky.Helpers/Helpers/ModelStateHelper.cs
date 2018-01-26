using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Husky
{
	public static class ModelStateHelper
	{
		public static List<string> GetAllErrorMessages(this ModelStateDictionary modelState) {
			List<string> errors = new List<string>();
			modelState.Values.Where(x => x.Errors.Any()).ToList().ForEach(state => {
				errors.AddRange(state.Errors.Select(x => x.ErrorMessage));
			});
			return errors;
		}

		public static Result ToResult(this ModelStateDictionary modelState) {
			var errors = modelState.GetAllErrorMessages();
			return new Result {
				Ok = errors.Count == 0,
				Message = errors.FirstOrDefault()
			};
		}

		public static IActionResult ToJsonResult(this ModelStateDictionary modelState) {
			return modelState.ToResult().ToJsonResult();
		}
	}
}
