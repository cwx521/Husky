using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Husky.AspNetCore
{
	#region Result,  Result<T>

	public class Result
	{
		public Result(bool ok = false, string message = null) {
			Ok = ok;
			Message = message;
		}
		public virtual bool Ok { get; set; }
		public virtual string Message { get; set; }

		public string ToJson() {
			return JsonConvert.SerializeObject(this);
		}

		public JsonResult ToJsonResult() {
			return new JsonResult(this);
		}
	}

	public class Result<T> : Result
	{
		public Result(bool ok = false, string message = null, T data = default(T)) : base(ok, message) { Data = data; }
		public T Data { get; set; }
	}

	#endregion

	#region Success, Success<T>

	public class Success : Result
	{
		public Success() : base(true) { }
	}

	public class Success<T> : Result<T>
	{
		public Success() : base(true) { }
		public Success(T data) : base(true, null, data) { }
	}

	#endregion

	#region Failure, Failure<T>

	public class Failure : Result
	{
		public Failure(string message) : base(false, message) { }

		public static Failure NoPermission => new Failure("You account does not have permission to take this action");
	}

	public class Failure<T> : Result<T>
	{
		public Failure(string message) : base(false, message) { }
	}

	#endregion
}