using System.Collections.Generic;
using System.Text;

namespace Cito.Cat.Pairwise.Web.Helpers
{
    public static class StringEncodingHelper
    {
        private static readonly Dictionary<int, char> CharactersToMap = new Dictionary<int, char>
        {
            {130, '‚'},
            {131, 'ƒ'},
            {132, '„'},
            {133, '…'},
            {134, '†'},
            {135, '‡'},
            {136, 'ˆ'},
            {137, '‰'},
            {138, 'Š'},
            {139, '‹'},
            {140, 'Œ'},
            {145, '‘'},
            {146, '’'},
            {147, '“'},
            {148, '”'},
            {149, '•'},
            {150, '–'},
            {151, '—'},
            {152, '˜'},
            {153, '™'},
            {154, 'š'},
            {155, '›'},
            {156, 'œ'},
            {159, 'Ÿ'},
            {173, '-'}
        };
        
        public static string ConvertStringEncoding(string txt, Encoding srcEncoding, Encoding dstEncoding)
        {
            if (string.IsNullOrEmpty(txt))
            {
                return txt;
            }

            if (srcEncoding == null)
            {
                throw new System.ArgumentNullException(nameof(srcEncoding));
            }

            if (dstEncoding == null)
            {
                throw new System.ArgumentNullException(nameof(dstEncoding));
            }

            var srcBytes = srcEncoding.GetBytes(txt);
            var dstBytes = Encoding.Convert(srcEncoding, dstEncoding, srcBytes);
            return dstEncoding.GetString(dstBytes);
        }

        public static string ConvertFromWindowsToUnicode(string txt)
        {
            if (string.IsNullOrEmpty(txt))
            {
                return txt;
            }

            var sb = new StringBuilder();
            foreach (var c in txt)
            {
                var i = (int)c;

                if (i >= 130 && i <= 173 && CharactersToMap.TryGetValue(i, out var mappedChar))
                {
                    sb.Append(mappedChar);
                    continue;
                }

                sb.Append(c);
            }

            return sb.ToString();
        }
    }
}