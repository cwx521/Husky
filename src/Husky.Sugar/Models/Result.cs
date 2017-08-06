namespace Husky.Sugar
{
	#region Result,  Result<T>

	public abstract class Result
	{
		public Result(bool ok = false, string message = null) {
			Ok = ok;
			Message = message;
		}
		public Result(bool ok, string key, string message) {
			Ok = ok;
			Key = key;
			Message = message;
		}
		public virtual bool Ok { get; protected set; }
		public virtual string Key { get; set; }
		public virtual string Message { get; set; }
	}

	public abstract class Result<T> : Result
	{
		public Result(bool ok = false, string message = null, T data = default(T)) : base(ok, message) { Data = data; }
		public Result(bool ok, string key, string message, T data = default(T)) : base(ok, key, message) { Data = data; }
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
		public Failure(string key, string message) : base(false, key, message) { }
	}

	public class Failure<T> : Result<T>
	{
		public Failure(string message) : base(false, message) { }
		public Failure(string key, string message) : base(false, key, message) { }
	}

	#endregion
}