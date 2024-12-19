namespace OWP.Extensions;

public static class DictionaryExtensions
{
    public static TValue GetValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
    {
        if (dictionary is null)
        {
            throw new ArgumentNullException(nameof(dictionary));
        }
        return dictionary.GetValueOrDefault(key, defaultValue);
    }

    public static TValue LookupByKey<TValue>(this Dictionary<string, TValue> dict, string key, TValue defaultValue)
    {
        var matchedKey = dict.Keys.FirstOrDefault(x => key.IndexOf(x, StringComparison.InvariantCultureIgnoreCase) >= 0);

        return string.IsNullOrEmpty(matchedKey) ? defaultValue : dict[matchedKey];
    }
}