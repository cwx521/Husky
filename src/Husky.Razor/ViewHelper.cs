using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Husky.Razor
{
	public enum BulletIcon
	{
		None,
		Caret,
		Ok,
		Error
	}

	public static class ViewHelper
	{
		public static IActionResult Tell(this PageModel model, IEnumerable<string> tellMessages, BulletIcon bulletIcon = BulletIcon.None, string scheme = null, bool modalDismissButton = false) {
			var view = new ViewResult {
				ViewName = nameof(Tell),
				ViewData = model.ViewData
			};
			view.ViewData["TellMessages"] = tellMessages;
			view.ViewData["Scheme"] = scheme;
			view.ViewData["BulletIcon"] = bulletIcon;
			view.ViewData["ModalDismissButton"] = modalDismissButton;
			return view;
		}
		public static IActionResult Tell(this PageModel model, string tellMessage, BulletIcon bulletIcon = BulletIcon.None, string scheme = null) {
			return Tell(model, new[] { tellMessage }, bulletIcon, scheme, true);
		}

		public static IActionResult NotFound => View("NotFound");
		public static IActionResult Forbidden => View("Forbidden");
		public static IActionResult OutOfService => View("OutOfService");
		public static IActionResult AntiViolence => View("AntiViolence");
		static ViewResult View(string viewName) => new ViewResult { ViewName = viewName };
	}
}
