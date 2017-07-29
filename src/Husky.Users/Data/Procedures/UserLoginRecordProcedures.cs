using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Husky.Users.Data
{
	public static class UserLoginRecordProcedures
	{
		public static bool IsSuspendFurtherLoginAttemptionByFailureRecordsAnalysis(this UserDbContext userDb, Guid userId, TimeSpan withinTime, int maxAllowedAttemptionTimes = 5) {
			var array = userDb.UserLoginRecords
				.AsNoTracking()
				.Where(x => x.UserId == userId)
				.Where(x => x.CreateTime >= DateTime.Now.Subtract(withinTime))
				.Where(x => x.LoginResult != LoginResult.RejectedContinuousAttemption)
				.OrderByDescending(x => x.Id)
				.Select(x => x.LoginResult)
				.Take(maxAllowedAttemptionTimes)
				.ToArray();
			return array.Length == maxAllowedAttemptionTimes && array.All(x => x == LoginResult.ErrorPassword);
		}
	}
}