using System;
using System.Security.Cryptography;
using OI.MVC.Models;

namespace OI.MVC.Helpers
{
    public static class AccountHelpers
    {
        private const int SaltByteSize = 24;
        private const int HashByteSize = 20; // to match the size of the PBKDF2-HMAC-SHA-1 hash
        private const int Pbkdf2Iterations = 1000;

        public static AccountData HashPassword(string password)
        {
            var cryptoProvider = new RNGCryptoServiceProvider();
            var salt = new byte[SaltByteSize];
            cryptoProvider.GetBytes(salt);

            var hash = Pbkdf2(password, salt, Pbkdf2Iterations, HashByteSize);
            return new AccountData() {Salt = Convert.ToBase64String(salt), HashPassword = Convert.ToBase64String(hash)};
        }

        public static bool ValidatePassword(string password, string storedSalt, string storedPassword)
        {
            var salt = Convert.FromBase64String(storedSalt);
            var hash = Convert.FromBase64String(storedPassword);

            var testHash = Pbkdf2(password, salt, Pbkdf2Iterations, hash.Length);
            return SlowEquals(hash, testHash);
        }

        private static bool SlowEquals(byte[] a, byte[] b)
        {
            var diff = (uint)a.Length ^ (uint)b.Length;
            for (var i = 0; i < a.Length && i < b.Length; i++)
            {
                diff |= (uint)(a[i] ^ b[i]);
            }
            return diff == 0;
        }

        private static byte[] Pbkdf2(string password, byte[] salt, int iterations, int outputBytes)
        {
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt) {IterationCount = iterations};
            return pbkdf2.GetBytes(outputBytes);
        }
    }
}