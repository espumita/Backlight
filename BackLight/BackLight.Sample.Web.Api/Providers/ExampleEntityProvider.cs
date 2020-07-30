using System;
using System.Text.Json;
using System.Threading.Tasks;
using Backlight.Providers;
using Backlight.Sample.Web.Api.Entities;

namespace Backlight.Sample.Web.Api.Providers {
    public class ExampleEntityProvider : CreateProvider, ReadProvider, UpdateProvider {
        public async Task<string> Create<T>(T entity) {
            Console.WriteLine($"Created {entity}");
            return Guid.NewGuid().ToString();
        }

        public async Task<BacklightEntity> Read<T>(string entityId) where T : BacklightEntity {
            Console.WriteLine($"Readed {entityId}");
            return new ExampleEntity();
        }

        public async Task Update<T>(string entityId, T entity) {
            Console.WriteLine($"Updated {entityId},{entity}");
        }
    }
}