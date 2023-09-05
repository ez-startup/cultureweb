using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text;

namespace CultureWeb.Utility
{
    public static class SessionExtensions
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore, // Handle circular references
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            // Other settings if needed
        };

        public static void Set<T>(this ISession session, string key, T value)
        {
            string serializedValue = JsonConvert.SerializeObject(value, JsonSerializerSettings);
            session.SetString(key, serializedValue);
        }

        public static T Get<T>(this ISession session, string key)
        {
            string value = session.GetString(key);

            return value == null ? default(T) :
                JsonConvert.DeserializeObject<T>(value, JsonSerializerSettings);
        }
    }
}
