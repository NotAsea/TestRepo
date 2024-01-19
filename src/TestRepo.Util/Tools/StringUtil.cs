using System.Diagnostics.CodeAnalysis;

namespace TestRepo.Util.Tools;

public static class StringUtil
{
    public static bool NotNull([NotNullWhen(true)] this string? str) => !string.IsNullOrEmpty(str);
}