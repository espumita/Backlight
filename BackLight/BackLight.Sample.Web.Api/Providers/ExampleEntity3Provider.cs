using System;
using System.Threading.Tasks;
using Backlight.Providers;

namespace Backlight.Sample.Web.Api.Providers {
    public class ExampleEntity3Provider : ReadProvider {

        public async Task<T> Read<T>(string entityId) {
            Console.WriteLine("Readed Entity3");
            return default;
        }
    }
}