namespace Husky.Data.Abstractions
{
	public interface IDatabaseFinder
	{
		DatabaseProvider Provider { get; }
		string ConnectionString { get; }
	}
}
