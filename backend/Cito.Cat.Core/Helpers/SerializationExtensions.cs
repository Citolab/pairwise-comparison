using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using Ims.Cat.Models;

namespace Cito.Cat.Core.Helpers
{
    public static class SerializationExtensions
    {
        public static string ToJson(this object objectInstance)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                //IgnoreNullValues = true,
                Converters = {new JsonStringEnumMemberConverter(JsonNamingPolicy.CamelCase)}
            };
            return JsonSerializer.Serialize(objectInstance, options);
            // return JsonConvert.SerializeObject(objectInstance, Formatting.Indented,
            //     new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore});
        }

        /// <summary>
        /// Deserialize to the specified type.
        /// </summary>
        /// <param name="json"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T FromJson<T>(this string json)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                //IgnoreNullValues = true,
                Converters = {new JsonStringEnumMemberConverter(JsonNamingPolicy.CamelCase)}
            };

            return JsonSerializer.Deserialize<T>(json, options);
        }

        /// <summary>
        /// Try deserializing from json
        /// </summary>
        /// <param name="json"></param>
        /// <param name="deserialized"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool TryFromJson<T>(this string json, out T deserialized)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true,
                    //IgnoreNullValues = true,
                    Converters = {new JsonStringEnumMemberConverter(JsonNamingPolicy.CamelCase)}
                };
                deserialized = JsonSerializer.Deserialize<T>(json, options);
                return true;
            }
            catch
            {
                deserialized = default;
                return false;
            }
        }

        public static string XmlSerializeToString(this object objectInstance)
        {
            var serializer = new XmlSerializer(objectInstance.GetType());
            var sb = new StringBuilder();

            using (TextWriter writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, objectInstance);
            }

            return sb.ToString();
        }

        public static T XmlDeserializeFromString<T>(this string objectData)
        {
            return (T) XmlDeserializeFromString(objectData, typeof(T));
        }

        private static object XmlDeserializeFromString(this string objectData, Type type)
        {
            var serializer = new XmlSerializer(type);
            object result;

            using (TextReader reader = new StringReader(objectData))
            {
                result = serializer.Deserialize(reader);
            }

            return result;
        }

        /// <summary>
        /// Create MD5 hash of the string.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        // ReSharper disable once InconsistentNaming
        private static string CreateMD5(this string input)
        {
            // Use input string to calculate MD5 hash
            using System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // Convert the byte array to hexadecimal string
            var sb = new StringBuilder();
            foreach (var t in hashBytes)
            {
                sb.Append(t.ToString("X2"));
            }

            return sb.ToString();
        }

        public static string GetMD5Hash(this SectionDType sectionDType)
        {
            var hashableString = (sectionDType.QtiMetadata?.ToString() ?? "") + (sectionDType.QtiUsagedata ?? "") +
                                 (sectionDType.SectionConfiguration ?? "");
            return hashableString.CreateMD5();
        }

        public static void PadBase64Strings(this SessionDType sessionDType)
        {
            if (!string.IsNullOrWhiteSpace(sessionDType.Demographics))
            {
                sessionDType.Demographics = PadBase64String(sessionDType.Demographics);
            }

            if (!string.IsNullOrWhiteSpace(sessionDType.PersonalNeedsAndPreferences))
            {
                sessionDType.PersonalNeedsAndPreferences = PadBase64String(sessionDType.PersonalNeedsAndPreferences);
            }
        }

        public static void PadBase64Strings(this SectionDType sectionDType)
        {
            if (!string.IsNullOrWhiteSpace(sectionDType.QtiUsagedata))
            {
                sectionDType.QtiUsagedata = PadBase64String(sectionDType.QtiUsagedata);
            }

            if (!string.IsNullOrWhiteSpace(sectionDType.SectionConfiguration))
            {
                sectionDType.SectionConfiguration = PadBase64String(sectionDType.SectionConfiguration);
            }
        }

        /// <summary>
        /// Base64 encode the string.
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string Base64Encode(this string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static bool TryBase64Decode(this string base64EncodedData, out string decoded)
        {
            var input = PadBase64String(base64EncodedData);

            try
            {
                var base64EncodedBytes = Convert.FromBase64String(input);
                decoded= Encoding.UTF8.GetString(base64EncodedBytes);
                return true;
            }
            catch
            {
                decoded = default;
                return false;
            }
        }

        /// <summary>
        /// Base64 decode the string
        /// </summary>
        /// <param name="base64EncodedData"></param>
        /// <returns></returns>
        public static string Base64Decode(this string base64EncodedData)
        {
            // add padding if missing
            var input = PadBase64String(base64EncodedData);
            
            var base64EncodedBytes = Convert.FromBase64String(input);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        private static string PadBase64String(string input)
        {
            var result = input;
            // add padding if missing
            var remainder = result.Length % 4;
            switch (remainder)
            {
                case 2:
                    result += "==";
                    break;
                case 3:
                    result += "=";
                    break;
            }

            return result;
        }
    }
}