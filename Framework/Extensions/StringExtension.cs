using System;
using System.Text;
using System.Text.RegularExpressions;
using Framework.Utility;
using Newtonsoft.Json;

namespace Framework.Extensions
{
    public static class StringExtension
    {
        public static T DeserializeObject<T>(this string value)
        {
            return JsonConvert.DeserializeObject<T>(value, JsonSerializerSettingsHelper.GetJsonSerializerSettings());
        }

        public static bool IsExpired(this string token)
        {
            if (token == null || ("").Equals(token))
            {
                return true;
            }

            /***
             * Make string valid for FromBase64String
             * FromBase64String cannot accept '.' characters and only accepts stringth whose length is a multitude of 4
             * If the string doesn't have the correct length trailing padding '=' characters should be added.
             */
            int indexOfFirstPoint = token.IndexOf('.') + 1;
            String toDecode = token.Substring(indexOfFirstPoint, token.LastIndexOf('.') - indexOfFirstPoint);
            while (toDecode.Length % 4 != 0)
            {
                toDecode += '=';
            }

            //Decode the string
            string decodedString = Encoding.ASCII.GetString(Convert.FromBase64String(toDecode));

            //Get the "exp" part of the string
            Regex regex = new Regex("(\"exp\":)([0-9]{1,})");
            Match match = regex.Match(decodedString);
            long timestamp = Convert.ToInt64(match.Groups[2].Value);

            DateTime date = new DateTime(1970, 1, 1).AddSeconds(timestamp);
            DateTime compareTo = DateTime.UtcNow;

            int result = DateTime.Compare(date, compareTo);

            return result < 0;
        }
    }
}
