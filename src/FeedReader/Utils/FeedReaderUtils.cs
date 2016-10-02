using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace FeedReader.Utils
{
    public static class FeedReaderUtils
    {
        private static byte[] GetHash(string input)
        {
            HashAlgorithm hashAlg = SHA1.Create();
            return hashAlg.ComputeHash(Encoding.UTF8.GetBytes(input));
        }
        public static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
            {
                sb.Append(b.ToString("X2"));
            }
            return sb.ToString();
        }


        public static string ScrubHtml(String html)
        {
            string noHtml = Regex.Replace(html, @"<[^>]+>|&nbsp;", "").Trim();

            return Regex.Replace(noHtml, @"\s{2,}", " ");
        }
    }
}