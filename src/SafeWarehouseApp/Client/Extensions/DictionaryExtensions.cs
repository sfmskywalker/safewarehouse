using System.Collections.Generic;

namespace SafeWarehouseApp.Client.Extensions
{
    public static class DictionaryExtensions
    {
        public static T? GetEntry<T>(this IDictionary<string, T> dictionary, string? key) => key is not null and not "" && dictionary.ContainsKey(key) ? dictionary[key] : default;
    }
}