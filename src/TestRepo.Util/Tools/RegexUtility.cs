using System.Text.RegularExpressions;

namespace TestRepo.Util.Tools;

public static partial class RegexUtility
{
    [GeneratedRegex(Constant.EmailRegex, RegexOptions.CultureInvariant)]
    private static partial Regex EmailRegex();

    [GeneratedRegex(Constant.PasswordRegex, RegexOptions.CultureInvariant)]
    private static partial Regex PasswordRegex();

    public static bool VerifyEmail(string email) => EmailRegex().IsMatch(email);

    public static bool VerifyPassword(string password) => PasswordRegex().IsMatch(password);
}
