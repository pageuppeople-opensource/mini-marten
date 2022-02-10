using System.Collections.Concurrent;
using HashidsNet;

namespace MartenApi;

public static class HashIdManager
{
    // alphabet and separators are missing "lI1O0S5" to help with readability.
    private const string HashAlphabet = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNPQRTUVWXYZ2346789";
    private const string HashSeparators = "cfhstuCFHTU";
    private const int MinHashLength = 6;

    private const string SaltPrefix = "PupMartenApi__";
    private static readonly ConcurrentDictionary<string, Hashids> HashidsDictionary = new ConcurrentDictionary<string, Hashids>();

    public static Hashids CreateHasher(string type)
    {
        return new Hashids(
            $"{SaltPrefix}{type}".ToLowerInvariant(),
            MinHashLength,
            HashAlphabet,
            HashSeparators);
    }

    public static string Hash(this long id, string type)
    {
        return HashidsDictionary.GetOrAdd(type.ToLowerInvariant(), CreateHasher).EncodeLong(id);
    }

    public static string Hash<T>(this long id)
    {
        return id.Hash(typeof(T).Name);
    }

    public static bool TryUnHash(this string hash, string type, out long id)
    {
        try
        {
            id = HashidsDictionary.GetOrAdd(type.ToLowerInvariant(), CreateHasher).DecodeLong(hash)[0];
            return true;
        }
        catch (Exception)
        {
            id = 0;
            return false;
        }
    }

    public static bool TryUnHash<T>(this string hash, out long id)
    {
        return hash.TryUnHash(typeof(T).Name, out id);
    }
}