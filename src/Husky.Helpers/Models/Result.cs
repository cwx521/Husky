using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Husky
{
	public class Result : IActionResult
	{
		public Result() {
		}
		public Result(bool ok, string? message = null) {
			Ok = ok;
			Message = message;
		}

		public bool Ok { get; set; }
		public string? Message { get; set; }

		public virtual string ToJson() => JsonSerializer.Serialize(this, new JsonSerializerOptions(JsonSerializerDefaults.Web) {
			DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
		});

		public async Task ExecuteResultAsync(ActionContext context) => await new JsonResult(this).ExecuteResultAsync(context);
	}

	public class Result<T> : Result
	{
		public Result() {
		}
		public Result(bool ok, string? message = null) : base(ok, message) {
		}
		public Result(bool ok, string? message, T data) : base(ok, message) {
			Data = data;
		}
		public T Data { get; set; } = default!;
	}
}