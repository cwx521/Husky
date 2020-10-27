namespace Husky
{
	public class TextValue<T> where T : struct
	{
		public string Text { get; set; } = null!;
		public T Value { get; set; }
	}
}
