using System.IO;

namespace Husky.Mail
{
	public sealed class MailAttachment
	{
		public string Name { get; set; } = null!;
		public string ContentType { get; set; } = null!;
		public Stream ContentStream { get; set; } = null!;
	}
}
