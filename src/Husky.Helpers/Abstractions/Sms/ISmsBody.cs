using System.Collections.Generic;

namespace Husky.Sms
{
	public interface ISmsBody
	{
		/// <summary>
		/// Parameters will replace the placeholders from the Template
		/// </summary>
		Dictionary<string, string> Parameters { get; set; }

		/// <summary>
		/// Message content template
		/// </summary>
		string? Template { get; set; }

		/// <summary>
		/// In case of that most of the providers only allow to send predefined contents, 
		/// a TemplateCode represents a message Template, which predefined from a particular 3rd-party management system
		/// </summary>
		string? TemplateAlias { get; set; }

		/// <summary>
		/// Message content sign
		/// </summary>
		string? SignName { get; set; }
	}
}
