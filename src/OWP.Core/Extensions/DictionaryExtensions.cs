namespace OWP.Core.Extensions;

public static class DictionaryExtensions
{
    public static TValue GetValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
    {
        if (dictionary is null)
        {
            throw new ArgumentNullException(nameof(dictionary));
        }
        return dictionary.ContainsKey(key) ? dictionary[key] : defaultValue;
    }

    public static TValue LookupByKey<TKey, TValue>(this Dictionary<string, TValue> dict, string key, TValue defaultValue)
    {
        var matchedKey = dict.Keys.FirstOrDefault(x => key.IndexOf(x, StringComparison.InvariantCultureIgnoreCase) >= 0);

        if (string.IsNullOrEmpty(matchedKey))
        {
            return defaultValue;
        }

        return dict[matchedKey];
    }
}