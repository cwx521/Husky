using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Husky.TwoFactor
{
	public class TwoFactorController : Controller
	{
		public TwoFactorController(ITwoFactorManager twoFactor) {
			_twoFactor = twoFactor;
		}

		private readonly ITwoFactorManager _twoFactor;

		[HttpPost]
		[AllowAnonymous]
		[IgnoreAntiforgeryToken]
		[Route("~/twofactor/RequestVerCode")]
		public async Task<IActionResult> RequestVerCodeAsync(
			string sendTo,
			string? overrideContentTemplateWithArg0 = null,
			string? overrideTemplateCode = null,
			string? overrideSignName = null) {
			return await _twoFactor.SendCodeAsync(sendTo, overrideContentTemplateWithArg0, overrideTemplateCode, overrideSignName);
		}

		[HttpPost]
		[AllowAnonymous]
		[IgnoreAntiforgeryToken]
		[Route("~/twofactor/Precheck")]
		public async Task<IActionResult> PrecheckVerCodeAsync(string sentTo, string code, bool setIntoUsedWhenSuccess = false) {
			return await _twoFactor.VerifyCodeAsync(sentTo, code, setIntoUsedWhenSuccess);
		}
	}
}
