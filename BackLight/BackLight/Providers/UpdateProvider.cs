namespace Backlight.Providers {
    public interface UpdateProvider {
        void Update<T>(string entityId, T entity);
    }
}