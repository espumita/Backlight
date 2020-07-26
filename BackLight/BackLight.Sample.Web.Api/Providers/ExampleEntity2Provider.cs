using System;
using System.Threading.Tasks;
using Backlight.Providers;

namespace Backlight.Sample.Web.Api.Providers {
    public class ExampleEntity2Provider : CRUDProvider {

        public async Task Create<T>(T entity) {
            Console.WriteLine("Created Entity2");
        }

        public async Task<T> Read<T>(string entityId) {
            Console.WriteLine("Readed Entity2");
            return default;
        }

        public void Update<T>(string entityId, T entity) {
            Console.WriteLine("Updated Entity2");
        }

        public void Delete<T>(string entityId) {
            Console.WriteLine("Deleted Entity2");
        }
    }
}