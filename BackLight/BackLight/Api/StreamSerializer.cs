using System.IO;
using System.Threading.Tasks;

namespace Backlight.Api {
    public interface StreamSerializer {

        Task<string> EntityFrom(Stream stream);
        Task<string> EntityPayLoadFrom(Stream stream);
    }
}