using System;
using System.Text.Json;
using System.Threading.Tasks;
using Backlight.Providers;
using Backlight.Sample.Web.Api.Entities;

namespace Backlight.Sample.Web.Api.Providers {
    public class ExampleEntityProvider : CreateProvider, ReadProvider, UpdateProvider {
        public async Task<string> Create<T>(T entity) {
            Console.WriteLine("Created TypeName");
            return Guid.NewGuid().ToString();
        }

        public async Task<BacklightEntity> Read<T>(string entityId) where T : class, BacklightEntity {
            Console.WriteLine("Readed TypeName");
            var backlightEntity = new ExampleEntity();
            var serialize = JsonSerializer.Serialize(backlightEntity);
            return backlightEntity;
        }

        public async Task Update<T>(string entityId, T entity) {
            Console.WriteLine("Updated TypeName");
        }
    }
}