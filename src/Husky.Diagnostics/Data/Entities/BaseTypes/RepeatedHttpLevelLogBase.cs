using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Husky.Diagnostics.Data
{
	public abstract class RepeatedHttpLevelLogBase : HttpLevelLogBase
	{
		public int Repeated { get; set; } = 1;

		[DefaultValueSql("getdate()")]
		public DateTime LastTime { get; set; } = DateTime.Now;

		[StringLength(32), Unicode(false)]
		public string Md5Comparison { get; set; } = null!;


		public abstract void ComputeMd5Comparison();
	}
}
