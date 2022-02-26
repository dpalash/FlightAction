using JsonNet.ContractResolvers;
using Newtonsoft.Json;

namespace Framework.Utility
{
    public static class JsonSerializerSettingsHelper
    {
        public static JsonSerializerSettings GetJsonSerializerSettings()
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented,
                ContractResolver = new PrivateSetterContractResolver(),
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            };

            return jsonSerializerSettings;
        }
    }
}