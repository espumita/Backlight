using System;
using System.Threading.Tasks;
using Backlight.Providers;
using Backlight.Sample.Web.Api.Entities;

namespace Backlight.Sample.Web.Api.Providers {
    public class ExampleEntity3Provider : ReadProvider {

        public async Task<BacklightEntity> Read<T>(string entityId) where T : class, BacklightEntity {
            Console.WriteLine("Readed Entity3");
            return new ExampleEntity3();
        }
    }
}