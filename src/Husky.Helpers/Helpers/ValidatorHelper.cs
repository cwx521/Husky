using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Husky
{
	public static class ValidatorHelper
	{
		public static Result Validate<T>(T instance) {
			var validationResults = new List<ValidationResult>();
			Validator.TryValidateObject(instance, new ValidationContext(instance), validationResults);

			if ( validationResults.Count == 0 ) {
				return new Success();
			}
			return new Failure(validationResults.First().ErrorMessage);
		}
	}
}
