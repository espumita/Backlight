using System.IO;
using System.Threading.Tasks;

namespace Backlight.Api.Serialization {
    public class MemoryStreamSerializer : StreamSerializer {

        public async Task<string> EntityPayloadFrom(Stream stream) {
            return await GetBodyFrom(stream);
        }

        private static async Task<string> GetBodyFrom(Stream bodyStream) {
            var memoryStream = new MemoryStream();
            await bodyStream.CopyToAsync(memoryStream);
            var streamReader = new StreamReader(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            var rawBody = await streamReader.ReadToEndAsync();
            streamReader.Close();
            return rawBody;
        }

    }

}