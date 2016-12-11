// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HashPassword.cs" company="Praeses">
//   Copyrite © 2014
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Shared.Class
{
    using System;
    using System.Linq;
    using System.Security.Cryptography;

    /// <summary>
    /// HashPassword class.
    /// </summary>
    public static class HashPassword
    {
        #region Constants

        /// <summary>
        /// The hash size.
        /// </summary>
        private const int HashSize = 20;

        /// <summary>
        /// The integer size.
        /// </summary>
        private const int IntegerSize = 4;

        /// <summary>
        /// The iterations.
        /// </summary>
        private const int Iterations = 1000;

        /// <summary>
        /// The salt size.
        /// </summary>
        private const int SaltSize = 16;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Hash password into a base64 string.
        /// </summary>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <returns>
        /// The hashed password.
        /// </returns>
        public static string Hash(string password)
        {
            return Convert.ToBase64String(HashBytes(password));
        }

        /// <summary>
        /// Hash password into a byte array.
        /// </summary>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <returns>
        /// The hashed password.
        /// </returns>
        public static byte[] HashBytes(string password)
        {
            if (password == default(string))
            {
                password = string.Empty;
            }

            var salt = new byte[SaltSize];
            new RNGCryptoServiceProvider().GetBytes(salt);
            byte[] hash = new Rfc2898DeriveBytes(password, salt, Iterations).GetBytes(HashSize);
            byte[] iter = BitConverter.GetBytes(Iterations);

            var hashBytes = new byte[IntegerSize + SaltSize + HashSize];
            Array.Copy(iter, 0, hashBytes, 0, IntegerSize);
            Array.Copy(salt, 0, hashBytes, IntegerSize, SaltSize);
            Array.Copy(hash, 0, hashBytes, IntegerSize + SaltSize, HashSize);
            return hashBytes;
        }

        /// <summary>
        /// Verify password matches hashedPassword.
        /// </summary>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <param name="hashedPassword">
        /// The hashed password.
        /// </param>
        /// <returns>
        /// True if valid, else false.
        /// </returns>
        public static bool Verify(string password, string hashedPassword)
        {
            return VerifyBytes(password, Convert.FromBase64String(hashedPassword));
        }

        /// <summary>
        /// Verify password matches hashedBytes.
        /// </summary>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <param name="hashBytes">
        /// The hash bytes.
        /// </param>
        /// <returns>
        /// True if valid, else false.
        /// </returns>
        public static bool VerifyBytes(string password, byte[] hashBytes)
        {
            if (password == default(string))
            {
                password = string.Empty;
            }

            var hash = new byte[HashSize];
            var salt = new byte[SaltSize];

            Array.Copy(hashBytes, IntegerSize, salt, 0, SaltSize);
            Array.Copy(hashBytes, IntegerSize + SaltSize, hash, 0, HashSize);

            int iterations = BitConverter.ToInt32(hashBytes, 0);
            byte[] testHash = new Rfc2898DeriveBytes(password, salt, iterations).GetBytes(HashSize);

            return hash.SequenceEqual(testHash);
        }

        #endregion
    }
}