namespace Backlight.Providers {
    public interface ReadProvider {
        T Read<T>(string entityId) where T : new();
    }
}