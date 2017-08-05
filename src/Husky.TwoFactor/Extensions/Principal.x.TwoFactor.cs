using System;
using System.Linq;
using System.Threading.Tasks;
using Husky.Authentication.Abstractions;
using Husky.Mail.Abstractions;
using Husky.Sugar;
using Husky.TwoFactor.Data;

namespace Husky.TwoFactor.Extensions
{
	public sealed partial class PrincipalTwoFactorExtensions
	{
		public PrincipalTwoFactorExtensions(IPrincipal principal, IMailSender mailSender, TwoFactorDbContext twoFactorDb) {
			_my = principal;
			_mailSender = mailSender;
			_twoFactorDb = twoFactorDb;
		}

		readonly IPrincipal _my;
		readonly IMailSender _mailSender;
		readonly TwoFactorDbContext _twoFactorDb;

		public async Task<Result> RequestTwoFactorCode(string emailOrMobile, TwoFactorPurpose purpose) {
			if ( emailOrMobile == null ) {
				throw new ArgumentNullException(nameof(emailOrMobile));
			}

			var isEmail = emailOrMobile.IsEmail();
			var isMobile = emailOrMobile.IsMainlandMobile();

			if ( !isEmail && !isMobile ) {
				return new Failure("发送对象 '{0}' 格式无效。".Xslate(emailOrMobile));
			}
			var code = new TwoFactorCode {
				UserId = _my.Id<Guid>(),
				PassCode = new Random().Next(0, 1000000).ToString().PadLeft(6, '0'),
				Purpose = purpose,
				SentTo = emailOrMobile
			};
			_twoFactorDb.Add(code);
			await _twoFactorDb.SaveChangesAsync();

			if ( isEmail ) {
				await SendTwoFactorCodeEmail(emailOrMobile, code.PassCode);
			}
			if ( isMobile ) {
				await SendTwoFactorCodeShortMessage(emailOrMobile, code.PassCode);
			}
			return new Success();
		}

		public async Task<Result> VerifyTwoFactorCode(string emailOrMobile, TwoFactorPurpose purpose, string passCode, int withinMinutes = 20) {
			if ( emailOrMobile == null ) {
				throw new ArgumentNullException(nameof(emailOrMobile));
			}
			if ( passCode == null ) {
				throw new ArgumentException(nameof(passCode));
			}

			var record = _twoFactorDb.TwoFactorCodes
				.Where(x => x.IsUsed == false)
				.Where(x => x.CreatedTime > DateTime.Now.AddMinutes(0 - withinMinutes))
				.Where(x => x.Purpose == purpose)
				.Where(x => x.SentTo == emailOrMobile)
				.Where(x => _my.IsAnonymous || x.UserId == _my.Id<Guid>())
				.OrderByDescending(x => x.CreatedTime)
				.Take(1)
				.FirstOrDefault();

			if ( record == null || string.Compare(passCode, record.PassCode, true) != 0 ) {
				return new Failure("验证码不正确。");
			}

			record.IsUsed = true;
			await _twoFactorDb.SaveChangesAsync();
			return new Success();
		}

		private async Task SendTwoFactorCodeEmail(string recipient, string passCode) {
			await _mailSender.Send(recipient, "验证码".Xslate(), passCode);
		}
		private Task SendTwoFactorCodeShortMessage(string mobileNumber, string passCode) => throw new NotImplementedException();
	}
}