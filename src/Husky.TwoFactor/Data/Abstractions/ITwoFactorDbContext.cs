using System;
using Microsoft.EntityFrameworkCore;

namespace Husky.TwoFactor.Data
{
	public interface ITwoFactorDbContext : IDisposable, IAsyncDisposable
	{
		DbContext Normalize();

		DbSet<TwoFactorCode> TwoFactorCodes { get; set; }
	}
}
