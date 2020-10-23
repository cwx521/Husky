namespace Husky
{
	public class Success : Result
	{
		public Success() : base(true) {
		}
		public Success(string? message) : base(true, message) {
		}
	}

	public class Success<T> : Result<T>
	{
		public Success() : base(true) {
		}
		public Success(T data) : base(true, null, data) {
		}
		public Success(string? message, T data) : base(true, message, data) {
		}

	}
}