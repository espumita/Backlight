using System.Threading.Tasks;

namespace Backlight.Providers {
    public interface CreateProvider {
        Task Create<T>(T entity);

    }

}