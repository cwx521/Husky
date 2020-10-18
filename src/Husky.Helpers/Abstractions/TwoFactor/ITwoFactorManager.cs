using System.Threading.Tasks;

namespace Husky.TwoFactor
{
	public interface ITwoFactorManager
	{
		Task<Result> SendCode(
			string mobileNumberOrEmailAddress,
			string? messageTemplateWithCodeArg0 = null,
			string? overrideAliyunSmsTemplateCode = null,
			string? overrideAliyunSmsSignName = null
		);

		Task<Result> SendCodeThroughAliyunSms(string mobileNumber, string? overrideAliyunSmsTemplateCode = null, string? overrideAliyunSmsSignName = null);

		Task<Result> SendCodeThroughEmail(string emailAddress, string? messageTemplateWithCodeArg0 = null);

		Task<Result> VerifyCode(string sentTo, string code, bool setIntoUsedAfterVerifying, int withinMinutes = 15);

		Task<Result> VerifyCode(ITwoFactorModel model, bool setIntoUsedAfterVerifying, int withinMinutes = 15);
	}
}