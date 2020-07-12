namespace Backlight {
    public interface DeleteProvider {
        void Delete<T>(string entityId);
    }
}