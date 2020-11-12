namespace Husky
{
	public class TextValuePair
	{
		public string Text { get; set; } = null!;
		public int Value { get; set; }
	}

	public class TextValuePair<T> where T : struct
	{
		public string Text { get; set; } = null!;
		public T Value { get; set; }
	}
}
