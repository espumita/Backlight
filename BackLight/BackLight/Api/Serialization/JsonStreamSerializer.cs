using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Backlight.Exceptions;

namespace Backlight.Api.Serialization {
    public class JsonStreamSerializer : StreamSerializer {

        private string body;

        public async Task<EntityRequestBody> EntityRequestBodyFrom(Stream stream) {
            body = await GetBodyFrom(stream);
            var typeName = TypeNameFrom(body);
            var payLoad = PayloadFrom(body);
            return new EntityRequestBody {
                TypeName = typeName,
                PayLoad = payLoad
            };
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

        private static string TypeNameFrom(string body) {
            EntityRequestBody entityRequestBody;
            try {
                entityRequestBody = JsonSerializer.Deserialize<EntityRequestBody>(body);
            } catch (Exception exception) {
                //TODO log
                throw new EntityDeserializationException();
            }
            if (string.IsNullOrEmpty(entityRequestBody.TypeName)) throw new EntityDeserializationException();
            return entityRequestBody.TypeName;
        }

        private static string PayloadFrom(string body) {
            try {
                var entityRequestBody = JsonSerializer.Deserialize<EntityRequestBody>(body);
                return entityRequestBody.PayLoad;
            } catch (Exception exception) {
                //TODO log
                throw new EntityDeserializationException();
            }
        }

    }

}