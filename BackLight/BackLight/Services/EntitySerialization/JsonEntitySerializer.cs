using System.Text.Json;

namespace Backlight.Services.EntitySerialization {
    public class JsonEntitySerializer : EntitySerializer {

        public string Serialize<T>(T entity) {
            return JsonSerializer.Serialize(entity);
        }

        public T Deserialize<T>(string entityPayload) {
            return JsonSerializer.Deserialize<T>(entityPayload);
        }
    }
}