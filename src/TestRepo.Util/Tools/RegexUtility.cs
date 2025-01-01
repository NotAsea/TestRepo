using System.Text.RegularExpressions;

namespace TestRepo.Util.Tools;

public static partial class RegexUtility
{
    [GeneratedRegex(Constant.EmailRegex, RegexOptions.CultureInvariant)]
    private static partial Regex EmailRegex();

    [GeneratedRegex(Constant.PasswordRegex, RegexOptions.CultureInvariant)]
    private static partial Regex PasswordRegex();

    private static readonly Regex CacheEmailRegex = EmailRegex();
    private static readonly Regex CachePasswordRegex = PasswordRegex();

    public static bool VerifyEmail(string email) => CacheEmailRegex.IsMatch(email);

    public static bool VerifyPassword(string password) => CachePasswordRegex.IsMatch(password);
}
