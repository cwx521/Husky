#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8603 // Possible null reference return.

using Microsoft.EntityFrameworkCore;

namespace Husky.Principal.UserMessages.Data
{
	public static class UserMessagesDbModelBuilderHelper
	{
		public static void OnUserMessagesDbModelCreating(this ModelBuilder mb) {
			mb.Entity<UserMessage>().HasQueryFilter(x => !x.IsDeleted);
			mb.Entity<UserMessage>(message => {
				message.HasOne(x => x.PublicContent).WithMany(x => x.UserMessages).HasForeignKey(x => x.PublicContentId);
			});
		}
	}
}
