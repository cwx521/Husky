using System;
using Microsoft.EntityFrameworkCore;

namespace Husky.Principal.UserMessages.Data
{
	public interface IUserMessagesDbContext : IDisposable, IAsyncDisposable
	{
		DbContext Normalize();

		DbSet<UserMessage> UserMessage { get; set; }
		DbSet<UserMessagePublicContent> UserMessagePublicContents { get; set; }
	}
}
