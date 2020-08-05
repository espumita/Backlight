using System;
using System.Text.Json;
using Backlight.Exceptions;

namespace Backlight.Services.EntitySerialization {
    public class JsonEntitySerializer : EntitySerializer {

        public string Serialize(object entity, Type returnType) {
            return JsonSerializer.Serialize(entity, returnType);
        }

        public object Deserialize(string entityPayload, Type returnType) {
            try {
                return JsonSerializer.Deserialize(entityPayload, returnType);
            } catch (Exception) {
                throw new EntityDeserializationException();
            }
        }
    }
}