using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Husky
{
	public class Result : IActionResult
	{
		public Result(bool ok = false, string? message = null) {
			Ok = ok;
			Message = message;
		}
		public virtual bool Ok { get; set; }
		public virtual string? Message { get; set; }

		public string ToJson() => JsonConvert.SerializeObject(this, new JsonSerializerSettings {
			NullValueHandling = NullValueHandling.Ignore
		});

		public async Task ExecuteResultAsync(ActionContext context) => await new JsonResult(this).ExecuteResultAsync(context);
	}

	public class Result<T> : Result
	{
		public Result(bool ok = false, string? message = null, T data = default) : base(ok, message) {
			Data = data;
		}
		public T Data { get; set; }
	}
}