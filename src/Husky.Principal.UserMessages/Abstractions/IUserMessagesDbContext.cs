#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8603 // Possible null reference return.

using Microsoft.EntityFrameworkCore;

namespace Husky.Principal.UserMessages.Data
{
	public interface IUserMessagesDbContext
	{
		DbContext Normalize();

		DbSet<UserMessage> UserMessage { get; set; }
		DbSet<UserMessagePublicContent> UserMessagePublicContents { get; set; }
	}
}
