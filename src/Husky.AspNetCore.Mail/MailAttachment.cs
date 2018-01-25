using System.IO;

namespace Husky.AspNetCore.Mail
{
	public sealed class MailAttachment
	{
		public string Name { get; set; }
		public Stream ContentStream { get; set; }
		public string ContentType { get; set; }
	}
}
