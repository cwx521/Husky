namespace Husky
{
	public class Failure : Result
	{
		public Failure(string? message) : base(false, message) {
		}
	}

	public class Failure<T> : Result<T>
	{
		public Failure(string? message) : base(false, message) {
		}
	}
}