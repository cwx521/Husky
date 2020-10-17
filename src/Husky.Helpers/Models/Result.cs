using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Husky
{
	#region Result,  Result<T>

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

	#endregion

	#region Success, Success<T>

	public class Success : Result
	{
		public Success() : base(true) { }
		public Success(string? message) : base(true, message) { }
	}

	public class Success<T> : Result<T>
	{
		public Success() : base(true) { }
		public Success(T data) : base(true, null, data) { }
		public Success(string? message, T data) : base(true, message, data) { }

	}

	#endregion

	#region Failure, Failure<T>

	public class Failure : Result
	{
		public Failure(string? message) : base(false, message) { }
	}

	public class Failure<T> : Result<T>
	{
		public Failure(string? message) : base(false, message) { }
	}

	#endregion
}