using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Backlight.Providers;
using Backlight.Sample.Web.Api.Entities;

namespace Backlight.Sample.Web.Api.Providers {
    public class ExampleEntity2Provider : CRUDProvider {

        public async Task<string> Create<T>(T entity) {
            var exampleEntity2 = entity as ExampleEntity2;
            Console.WriteLine($"Created {exampleEntity2.Name}");
            return Guid.NewGuid().ToString();
        }

        public async Task<object> Read<T>(string entityId, T returnType) {
            Console.WriteLine($"Readed {entityId}");
            return new ExampleEntity2 {
                Name = "George Lucas"
            };
        }

        public async Task Update<T>(string entityId, T entity) {
            var exampleEntity2 = entity as ExampleEntity2;
            Console.WriteLine($"Updated {exampleEntity2.Name} with id {entityId}");
        }

        public async Task Delete(string entityId) {
            Console.WriteLine($"Deleted {entityId}");
        }

        public async Task<List<string>> ReadAllIds() {
            return new List<string> {
                "1",
                "G",
                "2"
            };
        }
    }
}