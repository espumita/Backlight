using System;
using System.Threading.Tasks;
using Backlight.Providers;
using Backlight.Sample.Web.Api.Entities;

namespace Backlight.Sample.Web.Api.Providers {
    public class ExampleEntity2Provider : CRUDProvider {

        public async Task<string> Create<T>(T entity) {
            Console.WriteLine($"Created {entity}");
            return Guid.NewGuid().ToString();
        }

        public async Task<BacklightEntity> Read<T>(string entityId, T returnType) where T : Type {
            Console.WriteLine($"Readed {entityId}");
            return new ExampleEntity2();
        }

        public async Task Update<T>(string entityId, T entity) {
            Console.WriteLine($"Updated {entityId},{entity}");
        }

        public async Task Delete(string entityId) {
            Console.WriteLine($"Deleted {entityId}");
        }
    }
}