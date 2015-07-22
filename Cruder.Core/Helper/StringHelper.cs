using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Cruder.Helper
{
    public static class StringHelper
    {
        public static readonly Regex SlugRegex = new Regex(@"(^[a-z0-9])([a-z0-9_-]+)*([a-z0-9])$", RegexOptions.Compiled);

        public static string Split(this string text, int length, string suffix)
        {
            if (text.Length > length)
            {
                int num = length - suffix.Length;
                if (num > 0)
                {
                    text = text.Substring(0, num) + suffix;
                    return text;
                }
                text = text.Substring(0, length);
            }

            return text;
        }

        public static string GenerateRandom(int length)
        {
            StringBuilder generated = new StringBuilder();

            Random randomizer = new Random();
            List<int> possibleChars = new List<int>();
            possibleChars.AddRange(Enumerable.Range(48, 10)); //0-9
            possibleChars.AddRange(Enumerable.Range(65, 26)); //A-Z
            possibleChars.AddRange(Enumerable.Range(97, 26)); //a-z

            for (int i = 1; i <= length; i++)
            {
                generated.Append((char)possibleChars[randomizer.Next(possibleChars.Count)]);
            }

            return generated.ToString();
        }

        public static string GenerateSlug(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (SlugRegex.IsMatch(value))
            {
                return value;
            }

            string result = RemoveAccent(value).ToLowerInvariant();
            result = result.Trim('-', '.');
            result = result.Replace('.', '-');
            result = result.Replace("#", "-sharp");
            result = Regex.Replace(result, @"[^a-z0-9\s-]", string.Empty); // remove invalid characters
            result = Regex.Replace(result, @"\s+", " ").Trim(); // convert multiple spaces into one space

            return Regex.Replace(result, @"\s", "-"); // replace all spaces with hyphens
        }

        public static string Serialize(this IEnumerable<string> enumerable, string separator = ",")
        {
            string retVal = null;
            StringBuilder sb = new StringBuilder();

            foreach (string item in enumerable)
            {
                sb.AppendFormat("{0}{1}", item, separator);
            }

            if (!string.IsNullOrEmpty(sb.ToString()))
            {
                retVal = sb.Remove(sb.Length - separator.Length, separator.Length).ToString();
            }

            return retVal;
        }

        private static string RemoveAccent(string txt)
        {
            byte[] bytes = Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return Encoding.ASCII.GetString(bytes);
        }
    }
}
