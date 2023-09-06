namespace AnthonyPuppo.SemanticKernel.NL2EF.Extensions;

public static class StringExtensions
{
    public static string TrimStart(this string source, string prefix)
    {
        while (source.StartsWith(prefix))
        {
            source = source[prefix.Length..];
        }

        return source;
    }

    public static string TrimEnd(this string source, string suffix)
    {
        while (source.EndsWith(suffix))
        {
            source = source[..^suffix.Length];
        }

        return source;
    }
}
