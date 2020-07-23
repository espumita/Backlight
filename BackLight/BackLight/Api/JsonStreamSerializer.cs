using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Backlight.Exceptions;

namespace Backlight.Api {
    public class JsonStreamSerializer : StreamSerializer {

        //TODO unify this in a single body serialization at once

        private string body;

        public async Task<string> EntityFrom(Stream stream) {
            body = await GetBodyFrom(stream);
            return EntityFrom(body);
        }

        public async Task<string> EntityPayLoadFrom(Stream stream) {
         //   var body = await GetBodyFrom(stream);
            return EnitytPayLoadFrom(body);
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

        private static string EntityFrom(string body) {
            try {
                var backlightApiRequest = JsonSerializer.Deserialize<ApiRequest>(body);
                return backlightApiRequest.Entity;
            } catch (Exception exception) {
                //TODO log
                throw new EntityDeserializationException();
            }
        }

        private static string EnitytPayLoadFrom(string body) {
            try {
                var backlightApiRequest = JsonSerializer.Deserialize<ApiRequest>(body);
                return backlightApiRequest.PayLoad;
            } catch (Exception exception) {
                //TODO log
                throw new EntityDeserializationException();
            }
        }

    }
}