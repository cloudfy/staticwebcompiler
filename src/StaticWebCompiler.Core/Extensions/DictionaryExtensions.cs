namespace StaticWebCompiler.Extensions;

internal static class DictionaryExtensions
{
    internal static TValue? GetValueOrNull<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
    {
        if (dictionary.ContainsKey(key) == false)
            return default;

        return dictionary[key]!;
    }
}
