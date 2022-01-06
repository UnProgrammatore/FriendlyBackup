public static class Extensions
{
    public static string Concat(this IEnumerable<string> strings)
    {
        return string.Join("", strings);
    }
    public static string ToHexString(this IEnumerable<byte> bytes)
    {
        return bytes.Select(a => a.ToString("x2")).Concat();
    }
}