using System.IO;

namespace Husky.MailTo
{
	public sealed class MailAttachment
	{
		public string Name { get; set; }
		public Stream ContentStream { get; set; }
		public string ContentType { get; set; }
	}
}
