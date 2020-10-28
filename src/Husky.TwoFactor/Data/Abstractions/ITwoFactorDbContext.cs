using Microsoft.EntityFrameworkCore;

namespace Husky.TwoFactor.Data
{
	public interface ITwoFactorDbContext
	{
		DbContext Normalize();

		DbSet<TwoFactorCode> TwoFactorCodes { get; set; }
	}
}
