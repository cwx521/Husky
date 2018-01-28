using System;
using System.Linq;
using System.Threading.Tasks;
using Husky.AliyunSms;
using Husky.Mail;
using Husky.Principal;
using Husky.TwoFactor.Data;

namespace Husky.TwoFactor
{
	public sealed partial class TwoFactorManager
	{
		public TwoFactorManager(IPrincipalUser principal, TwoFactorDbContext twoFactorDb, IMailSender mailSender, AliyunSmsSender aliyunSmsSender) {
			_my = principal;
			_twoFactorDb = twoFactorDb;
			_mailSender = mailSender;
			_aliyunSmsSender = aliyunSmsSender;
		}

		readonly TwoFactorDbContext _twoFactorDb;
		readonly IPrincipalUser _my;

		readonly IMailSender _mailSender;
		readonly AliyunSmsSender _aliyunSmsSender;

		public async Task<Result> RequestTwoFactorCode(string emailOrMobile, string messageTemplateWithCodeAsArg0 = null) {
			if ( emailOrMobile == null ) {
				throw new ArgumentNullException(nameof(emailOrMobile));
			}

			var isEmail = emailOrMobile.IsEmail();
			var isMobile = emailOrMobile.IsMainlandMobile();

			if ( !isEmail && !isMobile ) {
				return new Failure($"无法发送到 '{emailOrMobile}' ");
			}

			var code = new TwoFactorCode {
				UserIdString = _my.IdString,
				Code = new Random().Next(0, 1000000).ToString().PadLeft(6, '0'),
				SentTo = emailOrMobile
			};
			_twoFactorDb.Add(code);
			await _twoFactorDb.SaveChangesAsync();

			if ( isEmail ) {
				var content = string.Format(messageTemplateWithCodeAsArg0, code.Code);
				await _mailSender.SendAsync("动态验证码", content, emailOrMobile);
			}
			else if ( isMobile ) {
				await _aliyunSmsSender.SendAsync(code.Code, emailOrMobile);
			}
			return new Success();
		}

		public async Task<Result> VerifyTwoFactorCode(TwoFactorModel model, bool setIntoUsedAfterVerifying, int codeWithinMinutes = 15) {
			if ( model == null ) {
				throw new ArgumentNullException(nameof(model));
			}

			var record = _twoFactorDb.TwoFactorCodes
				.Where(x => x.IsUsed == false)
				.Where(x => x.CreatedTime > DateTime.Now.AddMinutes(0 - codeWithinMinutes))
				.Where(x => x.SentTo == model.SendTo)
				.Where(x => _my.IsAnonymous || x.UserIdString == _my.IdString)
				.OrderByDescending(x => x.Id)
				.Take(1)
				.FirstOrDefault();

			if ( record == null || string.Compare(model.Code, record.Code, true) != 0 ) {
				return new Failure("验证码输入错误");
			}
			if ( setIntoUsedAfterVerifying ) {
				record.IsUsed = true;
				await _twoFactorDb.SaveChangesAsync();
			}
			return new Success();
		}
	}
}