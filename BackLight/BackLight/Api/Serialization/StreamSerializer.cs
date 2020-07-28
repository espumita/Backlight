using System.IO;
using System.Threading.Tasks;

namespace Backlight.Api.Serialization {
    public interface StreamSerializer {

        Task<EntityRequestBody> EntityRequestBodyFrom(Stream stream);
    }
}