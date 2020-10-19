using System.Collections.Generic;

namespace Husky.Sms
{
	public class SmsBody : ISmsBody
	{
		/// <summary>
		/// Parameters will replace the placeholders from the Template
		/// </summary>
		public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();

		/// <summary>
		/// Message content template
		/// </summary>
		public string? Template { get; set; }

		/// <summary>
		/// In case of that most of the providers only allow to send predefined contents, 
		/// a TemplateCode represents a message Template which predefined from a particular 3rd-party management system
		/// </summary>
		public string? TemplateAlias { get; set; }

		/// <summary>
		/// Message content sign
		/// </summary>
		public string? SignName { get; set; }
	}
}
