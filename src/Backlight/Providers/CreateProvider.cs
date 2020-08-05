using System.Threading.Tasks;

namespace Backlight.Providers {
    public interface CreateProvider {
        Task<string> Create<T>(T entity);

    }

}