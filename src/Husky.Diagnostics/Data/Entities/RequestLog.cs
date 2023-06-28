using Microsoft.EntityFrameworkCore;

namespace Husky.Diagnostics.Data
{
	[Index(nameof(Md5Comparison), IsUnique = false)]
	public class RequestLog : HttpLevelLogBase
	{
		public int Id { get; set; }

		public override void ComputeMd5Comparison() => Md5Comparison = Crypto.MD5(string.Concat(
			HttpMethod,
			Url,
			Data,
			AnonymousId,
			UserId,
			IsAjax,
			UserAgent,
			UserIp
		));
	}
}
