using System;

namespace Backlight.Services.EntitySerialization {
    public interface EntitySerializer {

        string Serialize(object entity, Type returnType);
        object Deserialize(string entityPayload, Type returnType);
    }
}