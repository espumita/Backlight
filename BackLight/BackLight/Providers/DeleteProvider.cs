namespace Backlight.Providers {
    public interface DeleteProvider {
        void Delete<T>(string entityId);
    }
}