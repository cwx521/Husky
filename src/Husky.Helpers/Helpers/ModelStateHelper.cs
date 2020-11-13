using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Husky
{
	public static class ModelStateHelper
	{
		public static List<string> GetAllErrorMessages(this ModelStateDictionary modelState) {
			return modelState.Values
				.SelectMany(x => x.Errors.Select(x => x.ErrorMessage))
				.ToList();
		}

		public static Result ToResult(this ModelStateDictionary modelState) {
			return new Result {
				Ok = modelState.ErrorCount == 0,
				Message = modelState.Values.SelectMany(x => x.Errors.Select(x => x.ErrorMessage)).FirstOrDefault()
			};
		}
	}
}
