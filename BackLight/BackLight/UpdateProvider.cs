namespace Backlight {
    public interface UpdateProvider {
        void Update<T>(string entityId, T entity);
    }
}