using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Husky
{
	#region Result,  Result<T>

	public class Result : IActionResult
	{
		public Result(bool ok = false, string message = null, int? code = null) {
			Ok = ok;
			Message = message;
			Code = code;
		}
		public virtual bool Ok { get; set; }
		public virtual string Message { get; set; }
		public virtual int? Code { get; set; }

		public string ToJson() {
			return JsonConvert.SerializeObject(this);
		}

		public async Task ExecuteResultAsync(ActionContext context) {
			await new JsonResult(this).ExecuteResultAsync(context);
		}
	}

	public class Result<T> : Result
	{
		public Result(bool ok = false, string message = null, int? code = null, T data = default(T)) : base(ok, message, code) { Data = data; }
		public T Data { get; set; }
	}

	#endregion

	#region Success, Success<T>

	public class Success : Result
	{
		public Success() : base(true) { }
		public Success(string message) : base(true, message, null) { }
	}

	public class Success<T> : Result<T>
	{
		public Success() : base(true) { }
		public Success(T data) : base(true, null, null, data) { }
		public Success(string message, T data) : base(true, message, null, data) { }

	}

	#endregion

	#region Failure, Failure<T>

	public class Failure : Result
	{
		public Failure(string message) : base(false, message) { }
		public Failure(string message, int code) : base(false, message, code) { }
	}

	public class Failure<T> : Result<T>
	{
		public Failure(string message) : base(false, message) { }
		public Failure(string message, int code) : base(false, message, code) { }
	}

	#endregion
}