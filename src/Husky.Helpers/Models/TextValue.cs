namespace Husky
{
	public class TextValue
	{
		public string Text { get; set; } = null!;
		public int Value { get; set; }
	}

	public class TextValue<T> where T : struct
	{
		public string Text { get; set; } = null!;
		public T Value { get; set; }
	}
}
