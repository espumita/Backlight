namespace Backlight.Services.EntitySerialization {
    public interface EntitySerializer {

        string Serialize<T>(T entity) where T : class;
        T Deserialize<T>(string entityPayload);
    }
}