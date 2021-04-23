using System;

namespace Cito.Cat.Core.Helpers
{
    public static class GuidExtensions
    {
        /// <summary>
        /// converts a GUID into a base64 string and shortens it a bit - so, url friendly version
        /// inspired by : https://madskristensen.net/post/A-shorter-and-URL-friendly-GUID
        /// </summary>
        /// <param name="guid"></param>
        /// <returns>Short, url-friendly string version of the specified Guid</returns>
        public static string ToShortGuidString(this Guid guid)
        {
            var enc = Convert.ToBase64String(guid.ToByteArray());
            enc = enc.Replace("/", "_");
            enc = enc.Replace("+", "-");
            return enc.Substring(0, 22);
        }
        
        public static bool TryDecodeToGuid(this string input, out Guid decoded)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(input))
                {
                    decoded = Guid.Empty;
                    return true;
                }

                input = input.Replace("_", "/");
                input = input.Replace("-", "+");
                byte[] buffer = Convert.FromBase64String(input + "==");
                decoded = new Guid(buffer);
                return true;
            }
            catch (Exception)
            {
                decoded = Guid.Empty;
                return false;
            }
        }
    }
}
