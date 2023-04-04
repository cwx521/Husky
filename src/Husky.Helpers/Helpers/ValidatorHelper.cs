using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;

namespace Husky
{
	public static class ValidatorHelper
	{
		public static Result Validate<T>(T dataModel) {
			if ( dataModel == null ) {
				throw new ArgumentNullException(nameof(dataModel));
			}

			var validationResults = new List<ValidationResult>();

			if ( !Validator.TryValidateObject(dataModel, new ValidationContext(dataModel), validationResults, true) ) {
				return new Failure(validationResults.First().ErrorMessage);
			}
			return new Success();
		}

		public static Result ValidateProperty<T, TProperty>(T dataModel, Expression<Func<T, TProperty>> propertyToValidate) {
			if ( dataModel == null ) {
				throw new ArgumentNullException(nameof(dataModel));
			}

			var propertyName = propertyToValidate.Body.ToString().RightBy(".", true);
			var propertyValue = propertyToValidate.Compile().Invoke(dataModel);
			var validationContext = new ValidationContext(dataModel) { MemberName = propertyName };
			var validationResults = new List<ValidationResult>();

			if ( !Validator.TryValidateProperty(propertyValue, validationContext, validationResults) ) {
				return new Failure(validationResults.First().ErrorMessage);
			}
			return new Success();
		}
	}
}
