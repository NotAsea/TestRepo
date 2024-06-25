using System.Security.Cryptography;
using Cysharp.Text;
using JetBrains.Annotations;
using Open.Text;

namespace TestRepo.Util.Tools;

[PublicAPI]
public static class SecretHasher
{
    private const int SaltSize = 16; // 128 bits
    private const int KeySize = 32; // 256 bits
    private const int Iterations = 50000;
    private const char SegmentDelimiter = ':';
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

    /// <summary>
    ///     Hash <paramref name="input" /> using PBKDF2 derived key wih SHA256
    /// </summary>
    /// <param name="input">input string, often password</param>
    /// <returns>hash with embed salt, iteration, hash algorithm in the end</returns>
    /// <remarks>
    /// async version of <see cref="Hash"/>
    /// </remarks>
    public static ValueTask<string> HashAsync(string input) => ValueTask.FromResult(Hash(input));

    /// <summary>
    ///     Hash <paramref name="input" /> using PBKDF2 derived key wih SHA256
    /// </summary>
    /// <param name="input">input string, often password</param>
    /// <returns>hash with embed salt, iteration, hash algorithm in the end</returns>
    public static string Hash(string input)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(input, salt, Iterations, Algorithm, KeySize);
        return ZString.Join(
            SegmentDelimiter,
            Convert.ToHexString(hash),
            Convert.ToHexString(salt),
            Iterations.ToString(),
            Algorithm.ToString()
        );
    }

    /// <summary>
    ///     Verify <paramref name="input"></paramref> with <paramref name="hashString" />, this function uses
    ///     <see cref="CryptographicOperations" /> for fixed time equal to defend time guessing.
    /// </summary>
    /// <param name="input">often password to verify</param>
    /// <param name="hashString">a hash password hashed by <see cref="HashAsync" /></param>
    /// <returns>true if equal, false otherwise</returns>
    public static bool Verify(string input, string hashString)
    {
        var segments = hashString
            .SplitAsSegments(SegmentDelimiter, StringSplitOptions.RemoveEmptyEntries)
            .ToArray();
        var hash = Convert.FromHexString(segments[0]);
        var salt = Convert.FromHexString(segments[1]);
        var iterations = int.Parse(segments[2]);
        var algorithm = new HashAlgorithmName(segments[3].ToString());
        var inputHash = Rfc2898DeriveBytes.Pbkdf2(input, salt, iterations, algorithm, hash.Length);
        return CryptographicOperations.FixedTimeEquals(inputHash, hash);
    }

    /// <summary>
    ///   Verify <paramref name="input"></paramref> with <paramref name="hashString" />, this function uses
    ///   <see cref="CryptographicOperations" /> for fixed time equal to defend time guessing.
    /// </summary>
    /// <param name="input">often password to verify</param>
    /// <param name="hashString">a hash password hashed by <see cref="HashAsync" /></param>
    /// <returns>true if equal, false otherwise</returns>
    /// <remarks>
    ///  async version of <see cref="Verify"/>
    /// </remarks>
    public static ValueTask<bool> VerifyAsync(string input, string hashString) =>
        ValueTask.FromResult(Verify(input, hashString));
}
