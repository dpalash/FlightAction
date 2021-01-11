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
    }
}
