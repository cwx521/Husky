using System;
using Microsoft.EntityFrameworkCore;

namespace Husky.TwoFactor.Data
{
	public interface ITwoFactorDbContext : IDbContext, IDisposable, IAsyncDisposable
	{
		DbSet<TwoFactorCode> TwoFactorCodes { get; set; }
	}
}
