using System;
using Backlight.Providers;

namespace Backlight.Sample.Web.Api.Providers {
    public class ExampleEntityProvider : CreateProvider, ReadProvider, UpdateProvider {
        public void Create<T>(T entity) {
            Console.WriteLine("Created Entity");
        }

        public T Read<T>(string entityId) {
            Console.WriteLine("Readed Entity");
            return default;
        }

        public void Update<T>(string entityId, T entity) {
            Console.WriteLine("Updated Entity");
        }
    }
}