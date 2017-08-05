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

		public async Task<Result<TwoFactorCode>> RequestTwoFactorCode(string emailOrMobile, TwoFactorPurpose purpose) {
			if ( emailOrMobile == null ) {
				throw new ArgumentNullException(nameof(emailOrMobile));
			}

			var isEmail = emailOrMobile.IsEmail();
			var isMobile = emailOrMobile.IsMainlandMobile();

			if ( !isEmail && !isMobile ) {
				return new Failure<TwoFactorCode>("发送对象 '{0}' 格式无效。".Xslate(emailOrMobile));
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

			return new Success<TwoFactorCode>(code);
		}

		public bool VerifyTwoFactorCode(string emailOrMobile, TwoFactorPurpose purpose, string passCode, int withinMinutes = 20) {
			if ( emailOrMobile == null ) {
				throw new ArgumentNullException(nameof(emailOrMobile));
			}
			if ( passCode == null ) {
				throw new ArgumentException(nameof(passCode));
			}

			var query = _twoFactorDb.TwoFactorCodes
				.Where(x => x.CreatedTime > DateTime.Now.AddMinutes(0 - withinMinutes))
				.Where(x => x.Purpose == purpose)
				.Where(x => x.SentTo == emailOrMobile);

			if ( _my.IsAuthenticated ) {
				query = query.Where(x => x.Id == _my.Id<Guid>());
			}

			var storedCode = query.OrderByDescending(x => x.CreatedTime).Select(x => x.PassCode).FirstOrDefault();
			return string.Compare(passCode, storedCode, true) == 0;
		}

		private async Task SendTwoFactorCodeEmail(string recipient, string passCode) {
			await _mailSender.Send(recipient, "验证码".Xslate(), passCode);
		}
		private Task SendTwoFactorCodeShortMessage(string mobileNumber, string passCode) => throw new NotImplementedException();
	}
}