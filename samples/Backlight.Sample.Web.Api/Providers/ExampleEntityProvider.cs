using System;
using System.Threading.Tasks;
using Backlight.Providers;
using Backlight.Sample.Web.Api.Entities;

namespace Backlight.Sample.Web.Api.Providers {
    public class ExampleEntityProvider : CreateProvider, ReadProvider, UpdateProvider {
        public async Task<string> Create<T>(T entity) {
            var exampleEntity = entity as ExampleEntity;
            Console.WriteLine($"Created {exampleEntity.Name}");
            return Guid.NewGuid().ToString();
        }

        public async Task<object> Read<T>(string entityId, T returnType) {
            Console.WriteLine($"Readed {entityId}");
            return new ExampleEntity();
        }

        public async Task Update<T>(string entityId, T entity) {
            var exampleEntity = entity as ExampleEntity;
            Console.WriteLine($"Updated {exampleEntity.Name} with id {entityId}");
        }
    }
}