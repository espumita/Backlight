using System;
using System.Threading.Tasks;
using Backlight.Providers;
namespace Backlight.Sample.Web.Api.Providers {
    public class ExampleEntityProvider : CreateProvider, ReadProvider, UpdateProvider {
        public async Task<string> Create<T>(T entity) {
            Console.WriteLine("Created TypeName");
            return Guid.NewGuid().ToString();
        }

        public async Task<T> Read<T>(string entityId){
            Console.WriteLine("Readed TypeName");
            return default;
        }

        public async Task Update<T>(string entityId, T entity) {
            Console.WriteLine("Updated TypeName");
        }
    }
}