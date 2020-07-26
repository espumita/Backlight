using System.Threading.Tasks;

namespace Backlight.Providers {
    public interface UpdateProvider {
        Task Update<T>(string entityId, T entity);
    }
}