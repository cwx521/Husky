using System.Collections.Generic;

namespace Husky.KeyValues
{
	public interface IKeyValueManager
	{
		IEnumerable<string> AllKeys { get; }
		bool Exists(string key);

		string? Get(string key);
		string? GetOrAdd(string key, string? defaultValueIfNotExist);
		void AddOrUpdate(string key, string? value);
		void Save(string key, string? value);

		T Get<T>(string key, T defaultValue = default) where T : struct;
		T GetOrAdd<T>(string key, T defaultValueIfNotExist) where T : struct;
		void AddOrUpdate<T>(string key, T value) where T : struct;
		void Save<T>(string key, T value) where T : struct;

		void Reload();
		void SaveAll();
	}
}
