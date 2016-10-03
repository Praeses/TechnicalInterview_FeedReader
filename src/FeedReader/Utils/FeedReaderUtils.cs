using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace FeedReader.Utils
{

    /// <summary>
    /// Provides helper functions to be used throughtout the application
    /// </summary>
    public static class FeedReaderUtils
    {
        /// <summary>
        /// Generates an sha1 hash of the input
        /// </summary>
        /// <param name="input">Input to be hashed</param>
        /// <returns>byte[] array of the hash</returns>
        private static byte[] GetHash(string input)
        {
            HashAlgorithm hashAlg = SHA1.Create();
            return hashAlg.ComputeHash(Encoding.UTF8.GetBytes(input));
        }

        /// <summary>
        /// Returns a string representation of a sha1 hash of an input string
        /// </summary>
        /// <param name="inputString">String to be hashed</param>
        /// <returns>Sha1 hash of input string</returns>
        public static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
            {
                sb.Append(b.ToString("X2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Removes HTML tags from text.
        /// </summary>
        /// <param name="html">Text that might contain html</param>
        /// <returns>Text with html removed</returns>
        public static string ScrubHtml(String html)
        {
            if (html != null)
            {
                string noHtml = Regex.Replace(html, @"<[^>]+>|&nbsp;", "").Trim();

                return Regex.Replace(noHtml, @"\s{2,}", " ");
            }
            else
            {
                return String.Empty;
            }
           
        }
    }
}