using System.Collections.Generic;

namespace Cito.Cat.Core.Helpers
{
    public static class StringExtensions
    {
        public static string Enumerate(this IEnumerable<string> strings)
        {
            var result = string.Empty;
            var counter = 1;
            foreach (var s in strings)
            {
                result += $"{counter}. {s} ";
                counter++;
            }

            return result.Trim();
        }

        /// <summary>
        /// Return the word count in a string.
        /// https://stackoverflow.com/a/8784654/160608
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static int GetWordCount(this string text)
        {
            int wordCount = 0, index = 0;

            // skip whitespace until first word
            while (index < text.Length && char.IsWhiteSpace(text[index]))
                index++;

            while (index < text.Length)
            {
                // check if current char is part of a word
                while (index < text.Length && !char.IsWhiteSpace(text[index]))
                    index++;

                wordCount++;

                // skip whitespace until next word
                while (index < text.Length && char.IsWhiteSpace(text[index]))
                    index++;
            }

            return wordCount;
        }
    }
}