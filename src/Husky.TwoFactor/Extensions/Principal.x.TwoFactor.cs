using System;
using System.Linq;
using System.Threading.Tasks;
using Husky.Authentication.Abstractions;
using Husky.MailTo.Abstractions;
using Husky.Sugar;
using Husky.TwoFactor.Data;

namespace Husky.TwoFactor
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

		public async Task<Result<TwoFactorCode>> SendMeTwoFactorCode(Emobaile emobaile, TwoFactorPurpose purpose) {
			if ( emobaile == null ) {
				throw new ArgumentNullException(nameof(emobaile));
			}
			if ( !emobaile.IsValid ) {
				return new Failure<TwoFactorCode>("发送对象 '{0}' 格式无效。".Xslate(emobaile.AccountName));
			}
			var code = new TwoFactorCode {
				UserId = _my.Id<Guid>(),
				PassCode = new Random().Next(0, 1000000).ToString(),
				Purpose = purpose,
				SentTo = emobaile.AccountName,
				SentThrough = emobaile.Type.Value,
			};
			_twoFactorDb.Add(code);
			await _twoFactorDb.SaveChangesAsync();

			if ( emobaile.IsEmail ) {
				await SendTwoFactorCodeEmail(emobaile.AccountName, code.PassCode);
			}
			if ( emobaile.IsMobile ) {
				await SendTwoFactorCodeShortMessage(emobaile.AccountName, code.PassCode);
			}

			return new Success<TwoFactorCode>(code);
		}

		public bool VerifyTwoFactorCode(Emobaile emobaile, string passCode, int withinMinutes = 20) {
			if ( emobaile == null || !emobaile.IsValid ) {
				throw new ArgumentException(nameof(emobaile));
			}
			if ( passCode == null ) {
				throw new ArgumentException(nameof(passCode));
			}

			var query = _twoFactorDb.TwoFactorCodes
				.Where(x => x.CreatedTime > DateTime.Now.AddMinutes(0 - withinMinutes))
				.Where(x => x.SentTo == emobaile.AccountName);

			if ( _my.IsAuthenticated ) {
				query = query.Where(x => x.Id == _my.Id<Guid>());
			}

			var storedCode = query.OrderByDescending(x => x.CreatedTime).Select(x => x.PassCode).FirstOrDefault();
			return string.Compare(passCode, storedCode, true) == 0;
		}

		// todo: next work.
		private Task SendTwoFactorCodeEmail(string accountName, string passCode) => throw new NotImplementedException();
		private Task SendTwoFactorCodeShortMessage(string accountName, string passCode) => throw new NotImplementedException();
	}
}