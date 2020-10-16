using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;

namespace Husky
{
	public static class ValidatorHelper
	{
		public static Result<T> Validate<T>(T instance) {
			var validationResults = new List<ValidationResult>();

			if ( Validator.TryValidateObject(instance, new ValidationContext(instance), validationResults, true) ) {
				return new Success<T>(instance);
			}
			return new Failure<T>(validationResults.First().ErrorMessage);
		}

		public static Result<T> Validate<T, TProperty>(T instance, Expression<Func<T, TProperty>> propertyToValidate) {
			var propertyName = propertyToValidate.Body.ToString().RightBy(".", true);
			var propertyValue = propertyToValidate.Compile().Invoke(instance);
			var validationContext = new ValidationContext(instance) { MemberName = propertyName };
			var validationResults = new List<ValidationResult>();

			if ( Validator.TryValidateProperty(propertyValue, validationContext, validationResults) ) {
				return new Success<T>(instance);
			}
			return new Failure<T>(validationResults.First().ErrorMessage);
		}
	}
}
