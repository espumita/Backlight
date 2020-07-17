namespace Backlight.Providers {
    public interface DeleteProvider : Provider {
        void Delete<T>(string entityId);
    }
}