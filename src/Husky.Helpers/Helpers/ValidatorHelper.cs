using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;

namespace Husky
{
	public static class ValidatorHelper
	{
		public static Result Validate<T>(T instance) {
			if ( instance == null ) {
				throw new ArgumentNullException(nameof(instance));
			}

			var validationResults = new List<ValidationResult>();

			if ( !Validator.TryValidateObject(instance, new ValidationContext(instance), validationResults, true) ) {
				return new Failure(validationResults.First().ErrorMessage);
			}
			return new Success();
		}

		public static Result ValidateProperty<T, TProperty>(T instance, Expression<Func<T, TProperty>> propertyToValidate) {
			if ( instance == null ) {
				throw new ArgumentNullException(nameof(instance));
			}

			var propertyName = propertyToValidate.Body.ToString().RightBy(".", true);
			var propertyValue = propertyToValidate.Compile().Invoke(instance);
			var validationContext = new ValidationContext(instance) { MemberName = propertyName };
			var validationResults = new List<ValidationResult>();

			if ( !Validator.TryValidateProperty(propertyValue, validationContext, validationResults) ) {
				return new Failure(validationResults.First().ErrorMessage);
			}
			return new Success();
		}
	}
}
