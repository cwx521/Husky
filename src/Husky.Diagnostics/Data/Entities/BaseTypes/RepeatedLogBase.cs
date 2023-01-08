using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Husky.Diagnostics.Data
{
	public abstract class RepeatedLogBase : LogBase
	{
		public int Repeated { get; set; } = 1;

		[DefaultValueSql("getdate()")]
		public DateTime LastTime { get; set; } = DateTime.Now;

		[StringLength(32), Column(TypeName = "varchar(32)"), EnableIndex(IsUnique = false)]
		public string Md5Comparison { get; set; } = null!;


		public abstract void ComputeMd5Comparison();
	}
}
