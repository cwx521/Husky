using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Husky.NotificationTasks.Data
{
	public class NotificationTask
	{
		[Key]
		public int Id { get; set; }

		[StringLength(200), Column(TypeName = "varchar(200)")]
		public string ApiUrl { get; set; } = null!;

		[StringLength(4000), Column(TypeName = "varchar(4000)")]
		public string? ContentBody { get; set; }

		public PostContentType ContentType { get; set; }

		public NotificationTaskStatus Status { get; set; }

		public int AutomatedCount { get; set; }

		public int ManualAttemptedCount { get; set; }

		[StringLength(4000)]
		public string? ReceivedContent { get; set; }

		public DateTime? FirstTriedTime { get; set; }

		public DateTime? LastTriedTime { get; set; }

		public DateTime? ScheduleNextTime { get; set; } = DateTime.Now;

		[DefaultValueSql("getdate()")]
		public DateTime CreatedTime { get; init; } = DateTime.Now;
	}
}