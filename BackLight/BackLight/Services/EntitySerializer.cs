namespace Backlight.Services {
    public interface EntitySerializer {

        string Serialize<T>(T entity);
        T Deserialize<T>(string entityPayload);
    }
}