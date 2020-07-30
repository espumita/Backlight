using System;
using System.Threading.Tasks;
using Backlight.Providers;
using Backlight.Sample.Web.Api.Entities;

namespace Backlight.Sample.Web.Api.Providers {
    public class ExampleEntity2Provider : CRUDProvider {

        public async Task<string> Create<T>(T entity) {
            Console.WriteLine("Created Entity2");
            return Guid.NewGuid().ToString();
        }

        public async Task<BacklightEntity> Read<T>(string entityId) where T : class, BacklightEntity {
            Console.WriteLine("Readed Entity2");
            return new ExampleEntity2();
        }

        public async Task Update<T>(string entityId, T entity) {
            Console.WriteLine("Updated Entity2");
        }

        public async Task Delete<T>(string entityId) {
            Console.WriteLine("Deleted Entity2");
        }
    }
}