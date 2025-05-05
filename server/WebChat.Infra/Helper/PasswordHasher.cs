using System.Security.Cryptography;

namespace WebChat.Infra.Helper
{
    public static class PasswordHasher
    {
        public static byte[] GenerateSalt(int size = 16)
        {
            var salt = new byte[size];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);
            return salt;
        }

        public static byte[] HashPassword(string password, byte[] salt,
                                          int iterations = 100_000, int hashSize = 32)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(
                password,
                salt,
                iterations,
                HashAlgorithmName.SHA256
            );
            return pbkdf2.GetBytes(hashSize);
        }

        public static string ToBase64(byte[] data) => Convert.ToBase64String(data);
        public static byte[] FromBase64(string str) => Convert.FromBase64String(str);
    }
}
