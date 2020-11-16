using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Husky.Tests
{
	[TestClass()]
	public class ModelStateHelperTests
	{
		[TestMethod()]
		public void GetAllErrorMessagesTest() {
			var modelState = new ModelStateDictionary();
			modelState.AddModelError("A", "ErrorMessageA");
			modelState.AddModelError("A", "ErrorMessageA2");
			modelState.AddModelError("B", "ErrorMessageB");

			var errorMessages = modelState.GetAllErrorMessages();
			Assert.AreEqual(3, errorMessages.Count);
		}

		[TestMethod()]
		public void ToResultTest() {
			var modelState = new ModelStateDictionary();

			var result = modelState.ToResult();
			Assert.AreEqual(true, result.Ok);

			modelState.AddModelError("A", "ErrorMessageA");
			modelState.AddModelError("A", "ErrorMessageA2");
			modelState.AddModelError("B", "ErrorMessageB");

			result = modelState.ToResult();
			Assert.AreEqual(false, result.Ok);
			Assert.AreEqual("ErrorMessageA", result.Message);

		}
	}
}