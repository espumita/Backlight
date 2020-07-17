namespace Backlight.Providers {
    public interface ReadProvider : Provider {
        T Read<T>(string entityId) where T : new();
    }
}