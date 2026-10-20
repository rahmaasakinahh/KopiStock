using System.Security.Cryptography;
namespace KopiStock.Services;
public static class PasswordSecurity
{
    private const int Iterations = 120_000;
    public static string Hash(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(16);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, 32);
        return $"{Iterations}:{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
    }
    public static bool Verify(string password, string stored)
    {
        try
        {
            string[] parts = stored.Split(':');
            if (parts.Length != 3 || !int.TryParse(parts[0], out int count) || count < 10_000) return false;
            byte[] salt = Convert.FromBase64String(parts[1]);
            byte[] expected = Convert.FromBase64String(parts[2]);
            byte[] actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, count, HashAlgorithmName.SHA256, expected.Length);
            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }
        catch (FormatException) { return false; }
    }
}
