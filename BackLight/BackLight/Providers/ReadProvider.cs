using System.Threading.Tasks;

namespace Backlight.Providers {
    public interface ReadProvider {
        Task<T> Read<T>(string entityId);
    }
}