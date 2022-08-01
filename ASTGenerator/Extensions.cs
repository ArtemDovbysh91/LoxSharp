namespace GenerateAst;

static class Extensions
{
    internal static void Deconstruct<T>(this IEnumerable<T> sequence, out T first, out IEnumerable<T> rest)
    {
        first = sequence.FirstOrDefault();
        rest = sequence.Skip(1);
    }

    internal static void Deconstruct<T>(this IEnumerable<T> sequence, out T first, out T second, out IEnumerable<T> rest) => (first, (second, rest)) = sequence;

    internal static string ToUppercaseFirst(this string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return string.Empty;
        }

        return char.ToUpper(str[0]) + str.Substring(1);
    }

    internal static string Repeat(this string str, int count)
    {
        if (count <= 0 || string.IsNullOrEmpty(str))
        {
            return string.Empty;
        }

        return string.Concat(Enumerable.Repeat(str, count));
    }
}