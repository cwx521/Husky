using System.Collections.Generic;

namespace Husky.KeyValues
{
	public interface IKeyValueManager
	{
		IEnumerable<string> AllKeys { get; }
		bool Exists(string key);

		string? Get(string key);
		T Get<T>(string key, T defaultValue = default) where T : struct;

		string? GetOrAdd(string key, string? defaultValueIfNotExist);
		T GetOrAdd<T>(string key, T defaultValueIfNotExist) where T : struct;

		void AddOrUpdate(string key, string? value);
		void AddOrUpdate<T>(string key, T value) where T : struct;

		void Reload();

		void Save<T>(string key, T value) where T : struct;
		void Save(string key, string? value);
		void SaveAll();
	}
}
