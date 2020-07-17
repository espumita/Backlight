namespace Backlight.Providers {
    public interface UpdateProvider : Provider {
        void Update<T>(string entityId, T entity);
    }
}