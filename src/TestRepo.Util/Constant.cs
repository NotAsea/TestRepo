namespace TestRepo.Util;

public static class Constant
{
    public const string EmailRegex =
        @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";

    public const string PasswordRegex =
        @"^(?=.*[a-z].*[a-z])(?=.*[A-Z].*[A-Z])(?=.*\d.*\d)(?=.*\W.*\W)[a-zA-Z0-9\S]{9,}$";

    public const string ValueIsNull = "Expect {PropertyName} has value but receive nothing";

    public const string WrongEmailFormat = "{PropertyName} is not a valid email format";

    public const string WrongPasswordFormat =
        "At least one lowercase, uppercase, number, and symbol exist in a 8+ character length password";
}
