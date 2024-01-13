namespace TestRepo.Util.Tools;

public static class LinqSourceGenHelper
{
    private readonly struct PredicateString(string check) : IStructFunction<string, bool>
    {
        public bool Invoke(string arg) => check.Equals(arg);
    }

    public static bool ContainGenForStringEnumerable(
        this IEnumerable<string> source,
        string check
    ) => source.Gen().Any(new PredicateString(check));
}
