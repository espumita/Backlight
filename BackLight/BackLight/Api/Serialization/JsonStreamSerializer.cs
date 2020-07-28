using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Backlight.Exceptions;

namespace Backlight.Api.Serialization {
    public class JsonStreamSerializer : StreamSerializer {

        private string body;

        public async Task<EntityRequestBody> EntityRequestBodyFrom(Stream stream) {
            try {
                body = await GetBodyFrom(stream);
                var tryToGetEntityRequestBodyFrom = TryToGetEntityRequestBodyFrom(body);
                return tryToGetEntityRequestBodyFrom;
            } catch (Exception exception) when(!(exception is EntityRequestBodyDeserializationException)) {
                throw new EntityRequestBodyDeserializationException();
            }
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

        private static EntityRequestBody TryToGetEntityRequestBodyFrom(string body) {
            var entityRequestBody = JsonSerializer.Deserialize<EntityRequestBody>(body);
            if (string.IsNullOrEmpty(entityRequestBody.TypeName)) throw new EntityRequestBodyDeserializationException();
            if (string.IsNullOrEmpty(entityRequestBody.PayLoad)) throw new EntityRequestBodyDeserializationException();
            return entityRequestBody;
        }

    }

}