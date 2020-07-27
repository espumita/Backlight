using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Backlight.Exceptions;

namespace Backlight.Api.Serialization {
    public class JsonStreamSerializer : StreamSerializer {

        private string body;

        public async Task<EntityPayload> EntityPayloadFrom(Stream stream) {
            body = await GetBodyFrom(stream);
            var typeName = TypeNameFrom(body);
            var value = ValueFrom(body);
            return new EntityPayload {
                TypeName = typeName,
                PayLoad = value
            };
        }

        private static async Task<string> GetBodyFrom(Stream bodyStream) {
            var memoryStream = new MemoryStream();
            await bodyStream.CopyToAsync(memoryStream);
            var streamReader = new StreamReader(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            var readToEndAsync = await streamReader.ReadToEndAsync();
            streamReader.Close();
            return readToEndAsync;
        }

        private static string TypeNameFrom(string body) {
            try {
                var payload = JsonSerializer.Deserialize<EntityPayload>(body);
                return payload.TypeName;
            } catch (Exception exception) {
                //TODO log
                throw new EntityDeserializationException();
            }
        }

        private static string ValueFrom(string body) {
            try {
                var payload = JsonSerializer.Deserialize<EntityPayload>(body);
                return payload.PayLoad;
            } catch (Exception exception) {
                //TODO log
                throw new EntityDeserializationException();
            }
        }

    }

}