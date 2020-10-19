namespace Husky.Diagnostics.Data
{
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
