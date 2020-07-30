using System;
using System.Text.Json;
using Backlight.Exceptions;

namespace Backlight.Services.EntitySerialization {
    public class JsonEntitySerializer : EntitySerializer {

        public string Serialize<T>(T entity) {
            return JsonSerializer.Serialize(entity);
        }

        public T Deserialize<T>(string entityPayload) {
            try {
                return JsonSerializer.Deserialize<T>(entityPayload);
            } catch (Exception) {
                throw new EntityDeserializationException();
            }
        }
    }
}